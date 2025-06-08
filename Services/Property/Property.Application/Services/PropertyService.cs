using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        public PropertyManager(IPropertyUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            using (_unitOfWork)
            {
                var property = _mapper.Map<Property>(createPropertyDto);
                await _unitOfWork.PropertyRepository.AddAsync(property);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<PropertyDto>(property);
            }
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
                var newImages = updateDto.Images.Select(url => new PropertyImage
                {
                    Id = Guid.NewGuid(),
                    PropertyId = id,
                    ImageUrl = url
                }).ToList();

                await _unitOfWork.PropertyImageRepository.AddRangeAsync(newImages);
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
            using (_unitOfWork)
            {
                var property = await _unitOfWork.PropertyRepository.GetByIdWithRoomsAndImagesAsync(id);
                if (property == null)
                    throw new KeyNotFoundException($"Property with ID {id} not found.");

                _unitOfWork.RoomRepository.RemoveRange(property.Rooms);
                _unitOfWork.PropertyImageRepository.RemoveRange(property.Images);

                _unitOfWork.PropertyRepository.Delete(property);
                await _unitOfWork.CommitAsync();
            }
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
    }
}
