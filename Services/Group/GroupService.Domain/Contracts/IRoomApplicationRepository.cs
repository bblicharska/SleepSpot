using GroupService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Domain.Contracts
{
    public interface IRoomApplicationRepository
    {
        Task<RoomApplication?> GetByIdAsync(Guid applicationId);
        Task<IEnumerable<RoomApplication>> GetByListingIdAsync(Guid listingId);
        Task<IEnumerable<RoomApplication>> GetByUserIdAsync(Guid userId);
        Task AddAsync(RoomApplication application);
        Task UpdateAsync(RoomApplication application);
        Task DeleteAsync(RoomApplication application);
        Task<bool> HasPendingApplicationAsync(Guid listingId, Guid userId);
        Task<IEnumerable<RoomApplication>> GetByApplicantIdAsync(Guid applicantUserId);
    }
}
