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
    public class PropertyRepository : IPropertyRepository
    {
        private readonly PropertyDbContext _context;

        public PropertyRepository(PropertyDbContext context)
        {
            _context = context;
        }

        public async Task<Property> GetByIdAsync(Guid id)
        {
            return await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.Properties
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Properties
                .Where(p => p.OwnerId == ownerId)
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> SearchAsync(string location, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Properties.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                query = query.Where(p => p.Address.Contains(location));

            if (minPrice.HasValue)
                query = query.Where(p => p.PricePerMonth >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.PricePerMonth <= maxPrice.Value);

            return await query
                .Include(p => p.Images)
                .Include(p => p.Rooms)
                    .ThenInclude(r => r.Images) // ✅ Include Room Images
                .ToListAsync();
        }

        public async Task AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
        }

        public void Update(Property property)
        {
            _context.Properties.Attach(property);
            _context.Entry(property).State = EntityState.Modified;
        }

        public void Delete(Property property)
        {
            _context.Properties.Remove(property);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<Property> GetByIdWithRoomsAsync(Guid id)
        {
            return await _context.Properties
                .Include(p => p.Rooms)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property> GetByIdWithRoomsAndImagesAsync(Guid id)
        {
            return await _context.Properties
                .Include(p => p.Rooms)
                    .ThenInclude(r => r.Images) // ✅ Include Room Images
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Property>> GetByOwnerIdWithRoomsAndImagesAsync(Guid ownerId)
        {
            return await _context.Properties
                .Where(p => p.OwnerId == ownerId)
                .Include(p => p.Rooms)
                    .ThenInclude(r => r.Images) // ✅ Include Room Images
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetAllWithRoomsAndImagesAsync()
        {
            return await _context.Properties
                .Include(p => p.Rooms)
                    .ThenInclude(r => r.Images) // ✅ Include Room Images
                .Include(p => p.Images)
                .ToListAsync();
        }
    }

}
