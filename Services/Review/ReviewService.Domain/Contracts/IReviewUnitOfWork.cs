using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Domain.Contracts
{
    public interface IReviewUnitOfWork : IDisposable
    {
        IReviewRepository ReviewRepository { get; }

        void Commit();
        Task<int> CommitAsync();
    }
}
