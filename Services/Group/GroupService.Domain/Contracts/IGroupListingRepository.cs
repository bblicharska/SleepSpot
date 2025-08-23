using GroupService.Domain.Models;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Domain.Contracts
{
    public interface IGroupListingRepository
    {
        Task<GroupListing?> GetByIdAsync(Guid listingId);
        Task<IEnumerable<GroupListing>> GetByGroupIdAsync(Guid groupId);
        Task<IQueryable<GroupListing>> GetFilteredListingsQueryAsync(GroupListingQueryParams queryParams);
        Task AddAsync(GroupListing listing);
        Task UpdateAsync(GroupListing listing);
        Task DeleteAsync(GroupListing listing);
    }
}
