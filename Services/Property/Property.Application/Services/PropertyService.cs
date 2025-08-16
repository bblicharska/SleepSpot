using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using PropertyService.Application.Dto;
using PropertyService.Domain.Contracts;
using PropertyService.Domain.Enums;
using PropertyService.Domain.Models;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Application.Services
{
    public class PropertyManager : IPropertyService
    {
        private readonly IPropertyUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PropertyManager> _logger;
        private readonly IMapper _mapper;
        private readonly string _uploadsPath;

        public PropertyManager(IPropertyUnitOfWork unitOfWork, IWebHostEnvironment environment, IMapper mapper, ILogger<PropertyManager> logger)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
            _mapper = mapper;
            _logger = logger;
            _uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "properties");

            try
            {
                if (!Directory.Exists(_uploadsPath))
                {
                    Directory.CreateDirectory(_uploadsPath);
                    _logger.LogInformation("Created upload directory: {UploadPath}", _uploadsPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create upload directory");
                throw; // Albo obsłuż błąd inaczej
            }
        }

        public async Task<PropertyDto> GetPropertyByIdAsync(Guid id)
        {
            var property = await _unitOfWork.PropertyRepository.GetByIdWithRoomsAndImagesAsync(id);
            if (property == null)
                throw new KeyNotFoundException($"Property with ID {id} not found.");

            var propertyDto = _mapper.Map<PropertyDto>(property);

            return propertyDto;
        }

        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync(PropertySortBy sortBy = PropertySortBy.CreatedAt, SortDirection sortDirection = SortDirection.Descending)
        {
            var properties = await _unitOfWork.PropertyRepository.GetAllWithRoomsAndImagesAsync(sortBy, sortDirection);

            // Configure JSON serializer settings to handle circular references
            var options = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            // Log the serialized data for debugging
            var json = JsonConvert.SerializeObject(properties, options);
            _logger.LogInformation("Retrieved properties: {Json}", json);

            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }

        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(Guid ownerId, PropertySortBy sortBy = PropertySortBy.CreatedAt, SortDirection sortDirection = SortDirection.Descending)
        {
            var properties = await _unitOfWork.PropertyRepository.GetByOwnerIdWithRoomsAndImagesAsync(ownerId, sortBy, sortDirection);
            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }

        public async Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(PropertyFilterDto f)
        {
            var props = await _unitOfWork.PropertyRepository.SearchAsync(f);
            return _mapper.Map<IEnumerable<PropertyDto>>(props);
        }

        public async Task<IEnumerable<RoomFilterDto>> SearchAllRoomsAsync(RoomSearchFilterDto f)
        {
            // Build the query over all rooms
            var query = _unitOfWork.RoomRepository.BuildSearchQuery(f);

            // Execute with property information included (Property should already be included in BuildSearchQuery)
            var rooms = await query
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            // Map to RoomFilterDto with property information
            return rooms.Select(r => new RoomFilterDto
            {
                // Map room properties
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                PricePerMonth = r.PricePerMonth,
                AreaInSquareMeters = r.AreaInSquareMeters,
                AvailableSince = r.AvailableSince,
                Capacity = r.Capacity,
                IsAvailable = r.IsAvailable,
                Images = _mapper.Map<List<PropertyImageDto>>(r.Images),

                // Add property information
                PropertyId = r.Property.Id,
                PropertyName = r.Property.Name,
                PropertyAddress = r.Property.Address,
                PropertyOwnerId = r.Property.OwnerId
            });
        }

        public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
        {
            // Map base property (without rooms)
            var property = _mapper.Map<Property>(createPropertyDto);
            property.Id = Guid.NewGuid(); // ensure new ID
            property.CreatedAt = DateTime.UtcNow;

            // Handle rooms (if any)
            if (createPropertyDto.Rooms != null && createPropertyDto.Rooms.Any())
            {
                var rooms = _mapper.Map<List<Room>>(createPropertyDto.Rooms);
                foreach (var room in rooms)
                {
                    room.Id = Guid.NewGuid();
                    room.PropertyId = property.Id;
                    room.CreatedAt = DateTime.UtcNow;
                }
                property.Rooms = rooms;
            }

            await _unitOfWork.PropertyRepository.AddAsync(property);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<PropertyDto>(property);
        }

        public async Task<PropertyDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto updateDto)
        {
            var property = await _unitOfWork.PropertyRepository.GetByIdWithRoomsAndImagesAsync(id);
            if (property == null)
                throw new KeyNotFoundException($"Property with ID {id} not found.");

            // Update basic fields
            property.Name = updateDto.Name;
            property.Description = updateDto.Description;
            property.Address = updateDto.Address;
            property.PricePerMonth = updateDto.PricePerMonth;
            property.AreaInSquareMeters = updateDto.AreaInSquareMeters;
            property.AvailableSince = updateDto.AvailableSince;
            property.IsEntirePlaceRentable = updateDto.IsEntirePlaceRentable;

            _unitOfWork.PropertyRepository.Update(property);
            await _unitOfWork.CommitAsync();

            // Update images
            var existingImages = await _unitOfWork.PropertyImageRepository.GetImagesByPropertyIdAsync(id);

            foreach (var imageDto in updateDto.Images ?? new List<ImageUpdateDto>())
            {
                if (imageDto.ToDelete)
                {
                    var existingImage = existingImages.FirstOrDefault(img => img.ImageUrl == imageDto.Url);
                    if (existingImage != null)
                    {
                        await DeleteImageAsync(existingImage.Id); // also deletes file
                    }
                    continue;
                }

                if (imageDto.File != null)
                {
                    // New upload
                    var uploadedImage = await AddImageAsync(id, imageDto.File);
                    uploadedImage.IsPrimary = imageDto.IsPrimary;
                    uploadedImage.DisplayOrder = imageDto.DisplayOrder;
                    await _unitOfWork.PropertyImageRepository.UpdateImageAsync(uploadedImage);
                }
                else if (!string.IsNullOrWhiteSpace(imageDto.Url))
                {
                    // Existing image update
                    var existingImage = existingImages.FirstOrDefault(img => img.ImageUrl == imageDto.Url);
                    if (existingImage != null)
                    {
                        existingImage.IsPrimary = imageDto.IsPrimary;
                        existingImage.DisplayOrder = imageDto.DisplayOrder;
                        await _unitOfWork.PropertyImageRepository.UpdateImageAsync(existingImage);
                    }
                }
            }

            await _unitOfWork.CommitAsync();



            // Update rooms
            var existingRooms = property.Rooms.ToList();
            _unitOfWork.RoomRepository.RemoveRange(existingRooms); // full replace strategy
            await _unitOfWork.CommitAsync();

            if (updateDto.Rooms != null && updateDto.Rooms.Any())
            {
                var newRooms = _mapper.Map<List<Room>>(updateDto.Rooms);
                foreach (var room in newRooms)
                    room.PropertyId = id;

                await _unitOfWork.RoomRepository.AddRangeAsync(newRooms);
                await _unitOfWork.CommitAsync();
            }

            var updatedProperty = await _unitOfWork.PropertyRepository.GetByIdWithRoomsAndImagesAsync(id);
            return _mapper.Map<PropertyDto>(updatedProperty);
        }

        public async Task DeletePropertyAsync(Guid id)
        {
            var property = await _unitOfWork.PropertyRepository.GetByIdWithRoomsAndImagesAsync(id);
            if (property == null)
                throw new KeyNotFoundException($"Property with ID {id} not found.");

            // Usuń tylko obrazy bez przypisanego RoomId (czyli te przypisane bezpośrednio do Property)
            var directImages = property.Images
                .Where(img => img.RoomId == null)
                .ToList();

            _unitOfWork.PropertyImageRepository.RemoveRange(directImages);

            // Reszta (Rooms + ich obrazy) zostaną usunięte automatycznie przez kaskadowość
            _unitOfWork.PropertyRepository.Delete(property);
            await _unitOfWork.CommitAsync();
        }


        public async Task AddRoomToPropertyAsync(Guid propertyId, CreateRoomDto createRoomDto)
        {
            var property = await _unitOfWork.PropertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new KeyNotFoundException($"Property with ID {propertyId} not found.");

            var room = _mapper.Map<Room>(createRoomDto);
            room.PropertyId = propertyId;

            await _unitOfWork.RoomRepository.AddAsync(room);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<RoomDto>> GetRoomsForPropertyAsync(Guid propertyId, RoomSortBy sortBy = RoomSortBy.CreatedAt, SortDirection sortDirection = SortDirection.Descending)
        {
            var property = await _unitOfWork.PropertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new KeyNotFoundException($"Property with ID {propertyId} not found.");

            var rooms = await _unitOfWork.RoomRepository.GetRoomsByPropertyIdAsync(propertyId, sortBy, sortDirection);
            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task DeleteRoomAsync(Guid roomId)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException("Room not found.");

            _unitOfWork.RoomRepository.Remove(room);
            await _unitOfWork.CommitAsync();
        }

        public async Task RentRoomAsync(Guid roomId)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {roomId} not found.");

            if (!room.IsAvailable)
                throw new InvalidOperationException("Room is already rented.");

            room.IsAvailable = false;
            _unitOfWork.RoomRepository.Update(room);
            await _unitOfWork.CommitAsync();
        }

        public async Task<PropertyImage> AddImageAsync(Guid propertyId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            if (!IsValidImageFile(file))
                throw new ArgumentException("Invalid image file type");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                throw new ArgumentException("File size too large (max 5MB)");

            var fileName = GenerateUniqueFileName(file.FileName);
            var filePath = Path.Combine(_uploadsPath, fileName);

            try
            {
                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Get next display order
                var maxOrder = await _unitOfWork.PropertyImageRepository.GetMaxImageDisplayOrderAsync(propertyId);

                var propertyImage = new PropertyImage
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    ImageUrl = $"/uploads/properties/{fileName}",
                    OriginalFileName = file.FileName,
                    DisplayOrder = maxOrder + 1,
                    UploadedAt = DateTime.UtcNow
                };

                await _unitOfWork.PropertyImageRepository.AddImageAsync(propertyImage);
                return propertyImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for property {PropertyId}", propertyId);

                // Cleanup file if database save fails
                if (File.Exists(filePath)) File.Delete(filePath);
                throw;
            }
        }

        public async Task<List<PropertyImage>> AddMultipleImagesAsync(Guid propertyId, List<IFormFile> files)
        {
            var uploadedImages = new List<PropertyImage>();

            foreach (var file in files)
            {
                try
                {
                    var image = await AddImageAsync(propertyId, file);
                    uploadedImages.Add(image);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload image {FileName}", file.FileName);
                    // Continue with other files
                }
            }

            return uploadedImages;
        }

        public async Task<List<PropertyImage>> GetPropertyImagesAsync(Guid propertyId)
        {
            return await _unitOfWork.PropertyImageRepository.GetPropertyImagesAsync(propertyId);
        }

        public async Task<bool> DeleteImageAsync(Guid imageId)
        {
            var image = await _unitOfWork.PropertyImageRepository.GetImageByIdAsync(imageId);
            if (image == null) return false;

            try
            {
                // Delete physical file
                var fullPath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (File.Exists(fullPath)) File.Delete(fullPath);

                // Delete from database
                await _unitOfWork.PropertyImageRepository.DeleteImageAsync(imageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId}", imageId);
                return false;
            }
        }

        public async Task<bool> SetPrimaryImageAsync(Guid imageId)
        {
            var image = await _unitOfWork.PropertyImageRepository.GetImageByIdAsync(imageId);
            if (image == null) return false;

            // Remove primary flag from other images
            if (image.PropertyId.HasValue)
            {
                await _unitOfWork.PropertyImageRepository.ClearPrimaryImageFlagAsync(image.PropertyId.Value);
            }

            // Set this image as primary
            await _unitOfWork.PropertyImageRepository.SetImageAsPrimaryAsync(imageId);
            return true;
        }
        public async Task<RoomDto> GetRoomByIdAsync(Guid roomId)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdWithPropertyAndImagesAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {roomId} not found.");

            return _mapper.Map<RoomDto>(room);
        }

        public async Task<RoomWithPropertyDetailsDto?> GetRoomWithPropertyDetailsAsync(Guid roomId)
        {
            var room = await _unitOfWork.RoomRepository.GetRoomWithPropertyDetailsAsync(roomId);

            if (room == null)
                return null;

            // Mapowanie na DTO - bez danych User (będą pobrane w Gateway)
            var result = new RoomWithPropertyDetailsDto
            {
                // Dane pokoju
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                DetailedDescription = room.DetailedDescription,
                PricePerMonth = room.PricePerMonth,
                AreaInSquareMeters = room.AreaInSquareMeters,
                AvailableSince = room.AvailableSince,
                Capacity = room.Capacity,
                IsAvailable = room.IsAvailable,
                Images = room.Images?.Select(i => new PropertyImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    OriginalFileName = i.OriginalFileName,
                    IsPrimary = i.IsPrimary,
                    DisplayOrder = i.DisplayOrder
                }).ToList() ?? new List<PropertyImageDto>(),

                // Dane property
                PropertyId = room.PropertyId,
                PropertyName = room.Property?.Name,
                PropertyAddress = room.Property?.Address,
                OwnerId = room.Property?.OwnerId, // Zwracamy OwnerId dla Gateway

                // Inne pokoje w tej nieruchomości
                OtherRoomsInProperty = room.Property?.Rooms?
                    .Where(r => r.Id != roomId)
                    .Select(r => new RoomSummaryDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        PricePerMonth = r.PricePerMonth,
                        AreaInSquareMeters = r.AreaInSquareMeters,
                        AvailableSince = r.AvailableSince,
                        Capacity = r.Capacity,
                        IsAvailable = r.IsAvailable,
                        MainImage = r.Images?.OrderBy(img => img.DisplayOrder)
                                            .FirstOrDefault()?.ImageUrl
                    }).ToList() ?? new List<RoomSummaryDto>()
            };

            return result;
        }

        public async Task<List<PropertyImage>> AddMultipleRoomImagesAsync(Guid roomId, List<IFormFile> files)
        {
            var uploadedImages = new List<PropertyImage>();

            var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {roomId} not found.");

            foreach (var file in files)
            {
                try
                {
                    var image = await AddImageForRoomAsync(roomId, room.PropertyId, file);
                    uploadedImages.Add(image);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload room image {FileName}", file.FileName);
                }
            }

            return uploadedImages;
        }

        private bool IsValidImageFile(IFormFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid()}{extension}";
        }

        private async Task<PropertyImage> AddImageForRoomAsync(Guid roomId, Guid propertyId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            if (!IsValidImageFile(file))
                throw new ArgumentException("Invalid image file type");

            if (file.Length > 5 * 1024 * 1024) // 5MB
                throw new ArgumentException("File too large");

            var fileName = GenerateUniqueFileName(file.FileName);
            var filePath = Path.Combine(_uploadsPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var maxOrder = await _unitOfWork.PropertyImageRepository.GetMaxImageDisplayOrderAsync(propertyId);

                var roomImage = new PropertyImage
                {
                    Id = Guid.NewGuid(),
                    RoomId = roomId,
                    PropertyId = propertyId,
                    ImageUrl = $"/uploads/properties/{fileName}",
                    OriginalFileName = file.FileName,
                    DisplayOrder = maxOrder + 1,
                    UploadedAt = DateTime.UtcNow
                };

                await _unitOfWork.PropertyImageRepository.AddImageAsync(roomImage);
                return roomImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for room {RoomId}", roomId);
                if (File.Exists(filePath)) File.Delete(filePath);
                throw;
            }
        }

    }

}

