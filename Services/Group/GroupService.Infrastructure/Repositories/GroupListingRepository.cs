using GroupService.Domain.Contracts;
using GroupService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Dto;
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

        public async Task<IQueryable<GroupListing>> GetFilteredListingsQueryAsync(GroupListingQueryParams queryParams)
        {
            var query = _context.GroupListings
                .Where(l => l.Status == ListingStatus.Active)
                .Include(l => l.Group)
                .Include(l => l.Applications)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.PreferredCity))
            {
                query = query.Where(l => l.PreferredCity.ToLower().Contains(queryParams.PreferredCity.ToLower()));
            }

            if (queryParams.MinBudget.HasValue)
            {
                query = query.Where(l => l.MaxBudgetPerPerson >= queryParams.MinBudget.Value);
            }

            if (queryParams.MaxBudget.HasValue)
            {
                query = query.Where(l => l.MaxBudgetPerPerson <= queryParams.MaxBudget.Value);
            }

            if (queryParams.MinRoommates.HasValue)
            {
                query = query.Where(l => l.DesiredRoommatesCount >= queryParams.MinRoommates.Value);
            }

            if (queryParams.MaxRoommates.HasValue)
            {
                query = query.Where(l => l.DesiredRoommatesCount <= queryParams.MaxRoommates.Value);
            }

            if (queryParams.HasProperty.HasValue)
            {
                if (queryParams.HasProperty.Value)
                {
                    query = query.Where(l => l.PropertyId.HasValue);
                }
                else
                {
                    query = query.Where(l => !l.PropertyId.HasValue);
                }
            }

            if (queryParams.HasRoom.HasValue)
            {
                if (queryParams.HasRoom.Value)
                {
                    query = query.Where(l => l.RoomId.HasValue);
                }
                else
                {
                    query = query.Where(l => !l.RoomId.HasValue);
                }
            }

            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                var searchTerm = queryParams.SearchTerm.ToLower();
                query = query.Where(l =>
                    l.Title.ToLower().Contains(searchTerm) ||
                    l.Description.ToLower().Contains(searchTerm));
            }

            query = queryParams.SortBy.ToLower() switch
            {
                "title" => queryParams.SortOrder.ToLower() == "asc"
                    ? query.OrderBy(l => l.Title)
                    : query.OrderByDescending(l => l.Title),
                "maxbudgetperperson" => queryParams.SortOrder.ToLower() == "asc"
                    ? query.OrderBy(l => l.MaxBudgetPerPerson)
                    : query.OrderByDescending(l => l.MaxBudgetPerPerson),
                "desiredroommatescount" => queryParams.SortOrder.ToLower() == "asc"
                    ? query.OrderBy(l => l.DesiredRoommatesCount)
                    : query.OrderByDescending(l => l.DesiredRoommatesCount),
                "preferredcity" => queryParams.SortOrder.ToLower() == "asc"
                    ? query.OrderBy(l => l.PreferredCity)
                    : query.OrderByDescending(l => l.PreferredCity),
                _ => queryParams.SortOrder.ToLower() == "asc"
                    ? query.OrderBy(l => l.CreatedAt)
                    : query.OrderByDescending(l => l.CreatedAt)
            };

            return query;
        }

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
