using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Domain.Contracts
{
    public interface IPropertyUnitOfWork : IDisposable
    {
        IPropertyRepository PropertyRepository { get; }
        IPropertyImageRepository PropertyImageRepository { get; }

        void Commit();
        Task<int> CommitAsync();
    }
}
