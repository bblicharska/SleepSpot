using RentalService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Infrastructure
{
    public class RentalUnitOfWork : IRentalUnitOfWork
    {
        private readonly RentalDbContext _context;

        public IRentalAgreementRepository RentalAgreementRepository { get; }

        public RentalUnitOfWork(RentalDbContext context, IRentalAgreementRepository rentalAgreementRepository)
        {
            _context = context;
            RentalAgreementRepository = rentalAgreementRepository;
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
