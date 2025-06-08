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
    public class GroupListingRepository : IGroupListingRepository
    {
        private readonly GroupDbContext _context;

        public GroupListingRepository(GroupDbContext context)
        {
            _context = context;
        }

        public async Task<GroupListing?> GetByIdAsync(Guid listingId)
            => await _context.GroupListings.Include(l => l.Applications).FirstOrDefaultAsync(l => l.Id == listingId);

        public async Task<IEnumerable<GroupListing>> GetByGroupIdAsync(Guid groupId)
            => await _context.GroupListings.Where(l => l.GroupId == groupId).ToListAsync();

        public async Task<IEnumerable<GroupListing>> GetActiveListingsAsync()
            => await _context.GroupListings.Where(l => l.Status == ListingStatus.Active).ToListAsync();

        public async Task AddAsync(GroupListing listing)
        {
            await _context.GroupListings.AddAsync(listing);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GroupListing listing)
        {
            _context.GroupListings.Update(listing);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(GroupListing listing)
        {
            _context.GroupListings.Remove(listing);
            await _context.SaveChangesAsync();
        }
    }

}
