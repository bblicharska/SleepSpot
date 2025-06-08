using PropertyService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Domain.Contracts
{
    public interface IRoomRepository
    {
        Task<Room> GetByIdAsync(Guid id);
        Task<IEnumerable<Room>> GetByPropertyIdAsync(Guid propertyId);
        Task AddAsync(Room room);
        Task AddRangeAsync(IEnumerable<Room> rooms);
        void Update(Room room);
        void Remove(Room room);
        void RemoveRange(IEnumerable<Room> rooms);
    }

}
