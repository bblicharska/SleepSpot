using Microsoft.EntityFrameworkCore;
using PropertyService.Domain.Contracts;
using PropertyService.Domain.Enums;
using PropertyService.Domain.Models;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly PropertyDbContext _context;

        public RoomRepository(PropertyDbContext context)
        {
            _context = context;
        }

        public async Task<Room> GetByIdAsync(Guid id)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Room>> GetByPropertyIdAsync(Guid propertyId)
        {
            return await _context.Rooms
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task AddAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
        }

        public async Task AddRangeAsync(IEnumerable<Room> rooms)
        {
            await _context.Rooms.AddRangeAsync(rooms);
        }

        public void Update(Room room)
        {
            _context.Rooms.Update(room);
        }

        public void Remove(Room room)
        {
            _context.Rooms.Remove(room);
        }

        public void RemoveRange(IEnumerable<Room> rooms)
        {
            _context.Rooms.RemoveRange(rooms);
        }
        public async Task<Room> GetByIdWithPropertyAndImagesAsync(Guid id)
        {
            return await _context.Rooms
                .Include(r => r.Property)
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Room?> GetRoomWithPropertyDetailsAsync(Guid roomId)
        {
            return await _context.Rooms
                .Include(r => r.Property)
                .Include(r => r.Images)
                .Include(r => r.Property.Rooms)
                    .ThenInclude(room => room.Images)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<IEnumerable<Room>> GetRoomsByPropertyIdAsync(Guid propertyId)
        {
            return await _context.Rooms
                .Where(r => r.PropertyId == propertyId)
                .Include(r => r.Images)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByPropertyIdAsync(Guid propertyId, RoomSortBy sortBy, SortDirection sortDirection)
        {
            var query = _context.Rooms
                .Where(r => r.PropertyId == propertyId)
                .Include(r => r.Images);

            var sortedQuery = ApplySorting(query, sortBy, sortDirection);

            return await sortedQuery.ToListAsync();
        }

        public IQueryable<Room> BuildSearchQuery(RoomSearchFilterDto f)
        {
            var q = _context.Rooms
                            .Include(r => r.Images)
                            .Include(r => r.Property)
                            .AsQueryable();

            // Add location filtering by property address
            if (!string.IsNullOrWhiteSpace(f.Location))
                q = q.Where(r => EF.Functions.Like(r.Property.Address, $"%{f.Location}%"));

            if (f.MinPrice.HasValue)
                q = q.Where(r => r.PricePerMonth >= f.MinPrice.Value);

            if (f.MaxPrice.HasValue)
                q = q.Where(r => r.PricePerMonth <= f.MaxPrice.Value);

            if (f.MinArea.HasValue)
                q = q.Where(r => r.AreaInSquareMeters >= f.MinArea.Value);

            if (f.MaxArea.HasValue)
                q = q.Where(r => r.AreaInSquareMeters <= f.MaxArea.Value);

            if (f.MinCapacity.HasValue)
                q = q.Where(r => r.Capacity >= f.MinCapacity.Value);

            if (f.IsAvailable.HasValue)
                q = q.Where(r => r.IsAvailable == f.IsAvailable.Value);

            if (f.AvailableSince.HasValue)
                q = q.Where(r => r.AvailableSince.Date <= f.AvailableSince.Value.Date);

            return q;
        }

        private IQueryable<Room> ApplySorting(IQueryable<Room> query, RoomSortBy sortBy, SortDirection direction)
        {
            return sortBy switch
            {
                RoomSortBy.Name => direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.Name)
                    : query.OrderByDescending(r => r.Name),
                RoomSortBy.Price => direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.PricePerMonth)
                    : query.OrderByDescending(r => r.PricePerMonth),
                RoomSortBy.Area => direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.AreaInSquareMeters)
                    : query.OrderByDescending(r => r.AreaInSquareMeters),
                RoomSortBy.Capacity => direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.Capacity)
                    : query.OrderByDescending(r => r.Capacity),
                RoomSortBy.PropertyName => direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.Property.Name)
                    : query.OrderByDescending(r => r.Property.Name),
                RoomSortBy.PropertyAddress => direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.Property.Address)
                    : query.OrderByDescending(r => r.Property.Address),
                RoomSortBy.AvailableSince => direction == SortDirection.Ascending
               ? query.OrderBy(r => r.AvailableSince)
               : query.OrderByDescending(r => r.AvailableSince),
                RoomSortBy.CreatedAt => direction == SortDirection.Ascending
                    ? query.OrderBy(r => r.CreatedAt)
                    : query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };
        }
    }

}
