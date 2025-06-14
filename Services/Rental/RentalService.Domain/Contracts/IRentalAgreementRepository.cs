using RentalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Domain.Contracts
{
    public interface IRentalAgreementRepository
    {
        Task<RentalAgreement?> GetByIdAsync(Guid id);
        Task<IEnumerable<RentalAgreement>> GetAllAsync();
        Task<IEnumerable<RentalAgreement>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<RentalAgreement>> GetByGroupIdAsync(Guid groupId);
        Task<IEnumerable<RentalAgreement>> GetByPropertyIdAsync(Guid propertyId);
        Task<IEnumerable<RentalAgreement>> GetActiveAgreementsAsync();

        Task AddAsync(RentalAgreement agreement);
        void Update(RentalAgreement agreement);
        void Remove(RentalAgreement agreement);
    }
}
