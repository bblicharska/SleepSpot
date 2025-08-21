using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Application.Interfaces
{
    public interface IPropertyClient
    {
        Task<PropertyDto?> GetPropertyByIdAsync(Guid propertyId);
        Task<RoomWithPropertyDetailsDto?> GetRoomByIdAsync(Guid roomId);
        Task UpdateRoomAvailabilityAsync(Guid roomId, bool isAvailable, DateTime? availableSince = null);
        Task UpdatePropertyAvailabilityAsync(Guid propertyId, bool isAvailable, DateTime? availableSince = null);
    }
}
