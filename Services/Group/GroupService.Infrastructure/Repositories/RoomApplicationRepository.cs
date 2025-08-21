using GroupService.Domain.Contracts;
using GroupService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Infrastructure.Repositories
{
    public class RoomApplicationRepository : IRoomApplicationRepository
    {
        private readonly GroupDbContext _context;

        public RoomApplicationRepository(GroupDbContext context)
        {
            _context = context;
        }

        public async Task<RoomApplication?> GetByIdAsync(Guid applicationId)
            => await _context.RoomApplications.FindAsync(applicationId);

        public async Task<IEnumerable<RoomApplication>> GetByListingIdAsync(Guid listingId)
            => await _context.RoomApplications.Where(a => a.ListingId == listingId).ToListAsync();

        public async Task<IEnumerable<RoomApplication>> GetByUserIdAsync(Guid userId)
            => await _context.RoomApplications.Where(a => a.ApplicantUserId == userId).ToListAsync();

        public async Task AddAsync(RoomApplication application)
        {
            await _context.RoomApplications.AddAsync(application);
        }

        public async Task UpdateAsync(RoomApplication application)
        {
            _context.RoomApplications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RoomApplication application)
        {
            _context.RoomApplications.Remove(application);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasPendingApplicationAsync(Guid listingId, Guid userId)
            => await _context.RoomApplications.AnyAsync(a => a.ListingId == listingId && a.ApplicantUserId == userId && a.Status == ApplicationStatus.Pending);

        public async Task<IEnumerable<RoomApplication>> GetByApplicantIdAsync(Guid applicantUserId)
        {
            return await _context.RoomApplications
                .AsNoTracking()
                .Where(r => r.ApplicantUserId == applicantUserId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }

}
