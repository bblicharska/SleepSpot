using PropertyService.Domain.Enums;
using PropertyService.Domain.Models;
using Shared.Dto;
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
        Task<Room> GetByIdWithPropertyAndImagesAsync(Guid id);
        Task<Room?> GetRoomWithPropertyDetailsAsync(Guid roomId);
        IQueryable<Room> BuildSearchQuery(RoomSearchFilterDto f);
        Task<IEnumerable<Room>> GetRoomsByPropertyIdAsync(Guid propertyId, RoomSortBy sortBy, SortDirection sortDirection);
    }

}
