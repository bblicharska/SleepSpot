using PropertyService.Domain.Models;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PropertySortBy
{
    CreatedAt,
    Name,
    Price,
    Area,
    Address,
    AvailableSince
}
public enum SortDirection
{
    Ascending,
    Descending
}

namespace PropertyService.Domain.Contracts
{
    public interface IPropertyRepository
    {
        Task<Property> GetByIdAsync(Guid id);
        Task<IEnumerable<Property>> GetAllAsync();
        Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId);
        Task AddAsync(Property property);
        void Update(Property property);
        void Delete(Property property);
        Task<int> SaveChangesAsync();
        Task<Property?> GetByIdWithRoomsAndImagesAsync(Guid id);
        Task<Property> GetByIdWithRoomsAsync(Guid id);

        Task<IEnumerable<Property>> GetAllWithRoomsAndImagesAsync(PropertySortBy sortBy,  SortDirection sortDirection);
        Task<IEnumerable<Property>> GetByOwnerIdWithRoomsAndImagesAsync(Guid ownerId, PropertySortBy sortBy, SortDirection sortDirection);
        Task<IEnumerable<Property>> SearchAsync(PropertyFilterDto f);
    }
}
