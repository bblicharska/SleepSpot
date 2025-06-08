using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Domain.Contracts
{
    public interface IGroupUnitOfWork : IDisposable
    {
        IGroupRepository GroupRepository { get; }
        IGroupMemberRepository GroupMemberRepository{ get; }
        IGroupListingRepository GroupListingRepository { get; }
        IRoomApplicationRepository RoomApplicationRepository { get; }

        void Commit();
        Task<int> CommitAsync();
    }
}
