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

    }
}
