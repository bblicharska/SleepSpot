using PropertyService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Domain.Contracts
{
    public interface IPropertyRepository
    {
        Task<Property> GetByIdAsync(Guid id);
        Task<IEnumerable<Property>> GetAllAsync();
        Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<Property>> SearchAsync(string location, decimal? minPrice, decimal? maxPrice);
        Task AddAsync(Property property);
        void Update(Property property);
        void Delete(Property property);
        Task<int> SaveChangesAsync();
        Task<Property?> GetByIdWithRoomsAndImagesAsync(Guid id);
        Task<Property> GetByIdWithRoomsAsync(Guid id);

        Task<IEnumerable<Property>> GetAllWithRoomsAndImagesAsync();
        Task<IEnumerable<Property>> GetByOwnerIdWithRoomsAndImagesAsync(Guid ownerId);
    }
}
