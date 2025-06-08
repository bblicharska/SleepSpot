using GroupService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Domain.Contracts
{
    public interface IGroupRepository
    {
        Task<Group?> GetByIdAsync(Guid groupId);
        Task<IEnumerable<Group>> GetAllAsync();
        Task<IEnumerable<Group>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(Group group);
        Task<bool> ExistsAsync(Guid groupId);
    }
}
