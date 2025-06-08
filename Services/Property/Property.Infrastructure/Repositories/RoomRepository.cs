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
    }

}
