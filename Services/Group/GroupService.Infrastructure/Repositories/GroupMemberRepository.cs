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
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly GroupDbContext _context;

        public GroupMemberRepository(GroupDbContext context)
        {
            _context = context;
        }

        public async Task<GroupMember?> GetByIdAsync(Guid memberId)
            => await _context.GroupMembers.FindAsync(memberId);

        public async Task<IEnumerable<GroupMember>> GetByGroupIdAsync(Guid groupId)
            => await _context.GroupMembers.Where(m => m.GroupId == groupId).ToListAsync();

        public async Task<GroupMember?> GetByUserAndGroupIdAsync(Guid userId, Guid groupId)
            => await _context.GroupMembers.FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);

        public async Task AddAsync(GroupMember member)
        {
            await _context.GroupMembers.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(GroupMember member)
        {
            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserInGroupAsync(Guid userId, Guid groupId)
            => await _context.GroupMembers.AnyAsync(m => m.UserId == userId && m.GroupId == groupId);
    }

}
