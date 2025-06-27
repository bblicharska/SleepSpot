using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PropertyService.Application.Dto;
using PropertyService.Domain.Contracts;
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

            return _mapper.Map<PropertyDto>(property);
        }

        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
        {
            var properties = await _unitOfWork.PropertyRepository.GetAllWithRoomsAndImagesAsync();

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

        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(Guid ownerId)
        {
            var properties = await _unitOfWork.PropertyRepository.GetByOwnerIdWithRoomsAndImagesAsync(ownerId);
            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }

        public async Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(string location, decimal? minPrice, decimal? maxPrice)
        {
            var properties = await _unitOfWork.PropertyRepository.SearchAsync(location, minPrice, maxPrice);
            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }

        public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
        {
            var property = _mapper.Map<Property>(createPropertyDto);
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
            property.IsEntirePlaceRentable = updateDto.IsEntirePlaceRentable;

            _unitOfWork.PropertyRepository.Update(property);
            await _unitOfWork.CommitAsync();

            // Update images
            var oldImages = await _unitOfWork.PropertyImageRepository.GetImagesByPropertyIdAsync(id);
            _unitOfWork.PropertyImageRepository.RemoveRange(oldImages);
            await _unitOfWork.CommitAsync();

            if (updateDto.Images != null && updateDto.Images.Any())
            {
                foreach (var imageDto in updateDto.Images)
                {
                    if (imageDto.ToDelete)
                        continue;

                    string imageUrl;

                    if (imageDto.File != null)
                    {
                        var uploaded = await AddImageAsync(id, imageDto.File);
                        imageUrl = uploaded.ImageUrl;
                    }
                    else if (!string.IsNullOrWhiteSpace(imageDto.Url))
                    {
                        imageUrl = imageDto.Url;
                    }
                    else
                    {
                        continue; // brak danych do zapisania
                    }

                    var newImage = new PropertyImage
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = id,
                        ImageUrl = imageUrl,
                        IsPrimary = imageDto.IsPrimary,
                        DisplayOrder = imageDto.DisplayOrder
                    };

                    await _unitOfWork.PropertyImageRepository.AddImageAsync(newImage);
                }

                await _unitOfWork.CommitAsync();
            }


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

                _unitOfWork.RoomRepository.RemoveRange(property.Rooms);
                _unitOfWork.PropertyImageRepository.RemoveRange(property.Images);

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

        public async Task<IEnumerable<RoomDto>> GetRoomsForPropertyAsync(Guid propertyId)
        {
            var property = await _unitOfWork.PropertyRepository.GetByIdWithRoomsAsync(propertyId);
            if (property == null)
                throw new KeyNotFoundException($"Property with ID {propertyId} not found.");

            return _mapper.Map<IEnumerable<RoomDto>>(property.Rooms);
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
            await _unitOfWork.PropertyImageRepository.ClearPrimaryImageFlagAsync(image.PropertyId);

            // Set this image as primary
            await _unitOfWork.PropertyImageRepository.SetImageAsPrimaryAsync(imageId);
            return true;
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
    }

}

