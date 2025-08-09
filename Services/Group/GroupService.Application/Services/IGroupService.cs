using GroupService.Application.Dto;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDto>> GetAllGroupsAsync();
        Task<GroupDto?> GetGroupByIdAsync(Guid groupId);
        Task<Guid> CreateGroupAsync(CreateGroupDto dto);
        Task UpdateGroupAsync(Guid groupId, CreateGroupDto dto);
        Task DeleteGroupAsync(Guid groupId);
        Task<IEnumerable<GroupDto>> GetGroupsForUserAsync(Guid userId);

        Task<IEnumerable<GroupMemberDto>> GetMembersByGroupIdAsync(Guid groupId);
        Task AddMemberAsync(GroupMemberDto dto);
        Task RemoveMemberAsync(Guid memberId);

        Task<PagedResult<GroupListingDto>> GetPagedListingsAsync(GroupListingQueryParams queryParams);
        Task<GroupListingDto?> GetListingByIdAsync(Guid listingId);
        Task<IEnumerable<GroupListingDto>> GetListingsByGroupIdAsync(Guid groupId);
        Task<Guid> CreateListingAsync(CreateGroupListingDto dto);
        Task UpdateListingAsync(Guid listingId, CreateGroupListingDto dto);
        Task DeleteListingAsync(Guid listingId);

        Task<IEnumerable<RoomApplicationDto>> GetApplicationsByListingIdAsync(Guid listingId);
        Task<RoomApplicationDto?> GetApplicationByIdAsync(Guid applicationId);
        Task<Guid> CreateApplicationAsync(RoomApplicationDto dto);
        Task UpdateApplicationStatusAsync(Guid applicationId, string status);
        Task DeleteApplicationAsync(Guid applicationId);
    }
}
