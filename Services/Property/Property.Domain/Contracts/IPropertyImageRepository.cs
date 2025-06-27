using PropertyService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Domain.Contracts
{
    public interface IPropertyImageRepository
    {
        Task<List<PropertyImage>> GetImagesByPropertyIdAsync(Guid propertyId);
        void RemoveRange(IEnumerable<PropertyImage> images);
        Task AddRangeAsync(IEnumerable<PropertyImage> images);
        Task<PropertyImage> AddImageAsync(PropertyImage image);
        Task<List<PropertyImage>> GetPropertyImagesAsync(Guid propertyId);
        Task<PropertyImage> GetImageByIdAsync(Guid imageId);
        Task<bool> DeleteImageAsync(Guid imageId);
        Task<int> GetMaxImageDisplayOrderAsync(Guid propertyId);
        Task ClearPrimaryImageFlagAsync(Guid propertyId);
        Task SetImageAsPrimaryAsync(Guid imageId);
    }
}
