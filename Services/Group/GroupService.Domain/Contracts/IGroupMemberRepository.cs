using GroupService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Domain.Contracts
{
    public interface IGroupMemberRepository
    {
        Task<GroupMember?> GetByIdAsync(Guid memberId);
        Task<IEnumerable<GroupMember>> GetByGroupIdAsync(Guid groupId);
        Task<GroupMember?> GetByUserAndGroupIdAsync(Guid userId, Guid groupId);
        Task AddAsync(GroupMember member);
        Task RemoveAsync(GroupMember member);
        Task<bool> IsUserInGroupAsync(Guid userId, Guid groupId);
    }
}
