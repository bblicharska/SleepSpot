using PropertyService.Application.Dto;
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
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
        Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(string location, decimal? minPrice, decimal? maxPrice);
        Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto);
        Task<PropertyDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto updatePropertyDto);
        Task DeletePropertyAsync(Guid id);
    }
}
