using Microsoft.AspNetCore.Http;
using PropertyService.Application.Dto;
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
    public interface IPropertyService
    {
        Task<PropertyDto> GetPropertyByIdAsync(Guid id);
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync(PropertySortBy sortBy = PropertySortBy.CreatedAt, SortDirection sortDirection = SortDirection.Descending);
        Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto);
        Task<PropertyDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto updatePropertyDto);
        Task DeletePropertyAsync(Guid id);
        Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(Guid ownerId, PropertySortBy sortBy = PropertySortBy.CreatedAt, SortDirection sortDirection = SortDirection.Descending);
        Task DeleteRoomAsync(Guid roomId);
        Task<IEnumerable<RoomDto>> GetRoomsForPropertyAsync(Guid propertyId, RoomSortBy sortBy = RoomSortBy.CreatedAt, SortDirection sortDirection = SortDirection.Descending);
       Task AddRoomToPropertyAsync(Guid propertyId, CreateRoomDto dto);
        Task<List<PropertyImage>> AddMultipleRoomImagesAsync(Guid roomId, List<IFormFile> files);
        Task RentRoomAsync(Guid roomId);
        Task<PropertyImage> AddImageAsync(Guid propertyId, IFormFile file);
        Task<List<PropertyImage>> AddMultipleImagesAsync(Guid propertyId, List<IFormFile> files);
        Task<List<PropertyImage>> GetPropertyImagesAsync(Guid propertyId);
        Task<bool> DeleteImageAsync(Guid imageId);
        Task<bool> SetPrimaryImageAsync(Guid imageId);
        Task<RoomDto> GetRoomByIdAsync(Guid roomId);
        Task<RoomWithPropertyDetailsDto?> GetRoomWithPropertyDetailsAsync(Guid roomId);
        Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(PropertyFilterDto filters);
        Task<IEnumerable<RoomFilterDto>> SearchAllRoomsAsync(RoomSearchFilterDto f);
    }
}
