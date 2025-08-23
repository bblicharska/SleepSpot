using GroupService.Domain.Contracts;
using GroupService.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Infrastructure
{
    public class GroupUnitOfWork : IGroupUnitOfWork
    {
        private readonly GroupDbContext _context;

        public IGroupRepository GroupRepository { get; }
        public IGroupMemberRepository GroupMemberRepository { get; }
        public IGroupListingRepository GroupListingRepository { get; }
        public IRoomApplicationRepository RoomApplicationRepository { get; }

        public GroupUnitOfWork(GroupDbContext context, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, IGroupListingRepository groupListingRepository, IRoomApplicationRepository roomApplicationRepository)
        {
            _context = context;
            GroupRepository = groupRepository;
            GroupMemberRepository = groupMemberRepository;
            GroupListingRepository = groupListingRepository;
            RoomApplicationRepository = roomApplicationRepository;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
