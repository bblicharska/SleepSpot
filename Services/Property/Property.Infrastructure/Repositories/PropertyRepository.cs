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

        // Pobranie nieruchomości po ID
        public async Task<Property> GetByIdAsync(Guid id)
        {
            return await _context.Properties
                .Include(p => p.Images)  // This line ensures images are included
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property> GetByIdWithImagesAsync(Guid id)
        {
            return await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Pobranie wszystkich nieruchomości
        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.Properties
              .Include(p => p.Images)  // Optionally include images here if needed
              .ToListAsync();
        }

        // Pobranie nieruchomości według właściciela
        public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Properties
             .Where(p => p.OwnerId == ownerId)
             .Include(p => p.Images)  // Optionally include images
             .ToListAsync();
        }

        // Wyszukiwanie nieruchomości według lokalizacji i cen
        public async Task<IEnumerable<Property>> SearchAsync(string location, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Properties.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                query = query.Where(p => p.Address.Contains(location));

            if (minPrice.HasValue)
                query = query.Where(p => p.PricePerNight >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.PricePerNight <= maxPrice.Value);

            return await query.Include(p => p.Images)  // Include images if needed
                .ToListAsync();
        }

        // Dodanie nowej nieruchomości
        public async Task AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
        }

        // Aktualizacja istniejącej nieruchomości
        public void Update(Property property)
        {
            _context.Properties.Attach(property);
            _context.Entry(property).State = EntityState.Modified;
        }

        // Usunięcie nieruchomości
        public void Delete(Property property)
        {
            _context.Properties.Remove(property);
        }

        // Zapisanie zmian w bazie danych
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
