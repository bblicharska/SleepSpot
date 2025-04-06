using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IUserUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        Task CommitAsync();
    }
}
