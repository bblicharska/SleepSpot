using Microsoft.EntityFrameworkCore;
using PropertyService.Domain.Contracts;
using PropertyService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Infrastructure.Repositories
{
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly PropertyDbContext _context;
        public PropertyImageRepository(PropertyDbContext context)
        {
            _context = context;
        }

        public async Task<List<PropertyImage>> GetImagesByPropertyIdAsync(Guid propertyId)
        {
            return await _context.PropertyImages
                .Where(img => img.PropertyId == propertyId)
                .ToListAsync();
        }

        public void RemoveRange(IEnumerable<PropertyImage> images)
        {
            _context.PropertyImages.RemoveRange(images);
        }

        public async Task AddRangeAsync(IEnumerable<PropertyImage> images)
        {
            await _context.PropertyImages.AddRangeAsync(images);
        }

        public async Task<PropertyImage> AddImageAsync(PropertyImage image)
        {
            _context.PropertyImages.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<List<PropertyImage>> GetPropertyImagesAsync(Guid propertyId)
        {
            return await _context.PropertyImages
                .Where(pi => pi.PropertyId == propertyId)
                .OrderBy(pi => pi.DisplayOrder)
                .ToListAsync();
        }

        public async Task<PropertyImage> GetImageByIdAsync(Guid imageId)
        {
            return await _context.PropertyImages.FindAsync(imageId);
        }

        public async Task<bool> DeleteImageAsync(Guid imageId)
        {
            var image = await _context.PropertyImages.FindAsync(imageId);
            if (image == null) return false;

            _context.PropertyImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetMaxImageDisplayOrderAsync(Guid propertyId)
        {
            var maxOrder = await _context.PropertyImages
                .Where(pi => pi.PropertyId == propertyId)
                .MaxAsync(pi => (int?)pi.DisplayOrder);

            return maxOrder ?? 0;
        }

        public async Task ClearPrimaryImageFlagAsync(Guid propertyId)
        {
            var images = await _context.PropertyImages
                .Where(pi => pi.PropertyId == propertyId && pi.IsPrimary)
                .ToListAsync();

            foreach (var image in images)
            {
                image.IsPrimary = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SetImageAsPrimaryAsync(Guid imageId)
        {
            var image = await _context.PropertyImages.FindAsync(imageId);
            if (image != null)
            {
                image.IsPrimary = true;
                await _context.SaveChangesAsync();
            }
        }

    }
}
