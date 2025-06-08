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
    public class GroupRepository : IGroupRepository
    {
        private readonly GroupDbContext _context;

        public GroupRepository(GroupDbContext context)
        {
            _context = context;
        }

        public async Task<Group?> GetByIdAsync(Guid groupId)
            => await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == groupId);

        public async Task<IEnumerable<Group>> GetAllAsync()
            => await _context.Groups.Include(g => g.Members).ToListAsync();

        public async Task<IEnumerable<Group>> GetByUserIdAsync(Guid userId)
            => await _context.Groups
                            .Include(g => g.Members)
                            .Where(g => g.Members.Any(m => m.UserId == userId))
                            .ToListAsync();

        public async Task AddAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Group group)
        {
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid groupId)
            => await _context.Groups.AnyAsync(g => g.Id == groupId);
    }
}
