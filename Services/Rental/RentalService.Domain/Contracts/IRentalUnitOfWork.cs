using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Domain.Contracts
{
    public interface IRentalUnitOfWork
    {
        IRentalAgreementRepository RentalAgreementRepository { get; }
        void Commit();
        Task<int> CommitAsync();
    }
}
