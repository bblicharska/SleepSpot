using Microsoft.EntityFrameworkCore;
using NLog.Filters;
using PropertyService.Application.Dto;
using PropertyService.Domain.Contracts;
using PropertyService.Domain.Models;
using Shared.Dto;
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
                    .ThenInclude(r => r.Images) 
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

        public async Task<Property?> GetByIdWithRoomsAndImagesAsync(Guid id)
        {
            return await _context.Properties
                .Include(p => p.Rooms)
                    .ThenInclude(r => r.Images)
                .Include(p => p.Images.Where(i => i.RoomId == null)) // Only property images
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Property>> GetByOwnerIdWithRoomsAndImagesAsync(Guid ownerId, PropertySortBy sortBy, SortDirection sortDirection)
        {
            var query = _context.Properties
                .Where(p => p.OwnerId == ownerId)
                .Include(p => p.Rooms)
                    .ThenInclude(r => r.Images)
                .Include(p => p.Images.Where(i => i.RoomId == null)); // Only property images

            var sortedQuery = ApplySorting(query, sortBy, sortDirection);

            return await sortedQuery.ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetAllWithRoomsAndImagesAsync(PropertySortBy sortBy, SortDirection sortDirection)
        {
            var query = _context.Properties
                .Include(p => p.Rooms)
                    .ThenInclude(r => r.Images)
                .Include(p => p.Images.Where(i => i.RoomId == null)); // Only property images

            var sortedQuery = ApplySorting(query, sortBy, sortDirection);

            return await sortedQuery.ToListAsync();
        }

        public async Task<IEnumerable<Property>> SearchAsync(PropertyFilterDto f)
        {
            var q = _context.Properties
                            .Include(p => p.Rooms)
                            .Include(p => p.Images)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(f.Location))
                q = q.Where(p => EF.Functions.Like(p.Address, $"%{f.Location}%"));

            if (f.MinPrice.HasValue)
                q = q.Where(p => p.PricePerMonth >= f.MinPrice.Value);

            if (f.MaxPrice.HasValue)
                q = q.Where(p => p.PricePerMonth <= f.MaxPrice.Value);

            if (f.MinArea.HasValue)
                q = q.Where(p => p.AreaInSquareMeters >= f.MinArea.Value);

            if (f.MaxArea.HasValue)
                q = q.Where(p => p.AreaInSquareMeters <= f.MaxArea.Value);

            if (f.IsAvailable.HasValue)
                q = q.Where(p => p.isAvailable == f.IsAvailable.Value);

            if (f.AvailableSince.HasValue)
                q = q.Where(p => p.AvailableSince.Date <= f.AvailableSince.Value.Date);

            if (f.IsEntirePlaceRentable.HasValue)
                q = q.Where(p => p.IsEntirePlaceRentable == f.IsEntirePlaceRentable.Value);

            return await q.ToListAsync();
        }

        private IQueryable<Property> ApplySorting(IQueryable<Property> query, PropertySortBy sortBy, SortDirection direction)
        {
            return sortBy switch
            {
                PropertySortBy.Name => direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Name)
                    : query.OrderByDescending(p => p.Name),
                PropertySortBy.Price => direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.PricePerMonth)
                    : query.OrderByDescending(p => p.PricePerMonth),
                PropertySortBy.Area => direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.AreaInSquareMeters)
                    : query.OrderByDescending(p => p.AreaInSquareMeters),
                PropertySortBy.Address => direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.Address)
                    : query.OrderByDescending(p => p.Address),
                PropertySortBy.AvailableSince => direction == SortDirection.Ascending
                ? query.OrderBy(p => p.AvailableSince)
                : query.OrderByDescending(p => p.AvailableSince),
                PropertySortBy.CreatedAt => direction == SortDirection.Ascending
                    ? query.OrderBy(p => p.CreatedAt)
                    : query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };
        }
    }

}
