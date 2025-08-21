using RentalService.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Application.Services
{
    public interface IRentalService
    {
        Task<IEnumerable<RentalAgreementDto>> GetAllAsync();
        Task<RentalAgreementDto> GetByIdAsync(Guid id);
        Task<RentalAgreementDto> CreateAsync(CreateRentalAgreementDto dto);
        Task<RentalAgreementDto> UpdateAsync(Guid id, UpdateRentalAgreementDto dto);
        Task<IEnumerable<RentalAgreementDto>> GetActiveByUserIdAsync(Guid userId);
        Task<IEnumerable<RentalAgreementDto>> GetActiveByGroupIdAsync(Guid groupId);

        Task<bool> DeleteAsync(Guid id);
        Task<RentalAgreementDto?> ActivateRentalAsync(Guid rentalAgreementId);
        Task<RentalAgreementDto?> DeclineRentalAsync(Guid rentalAgreementId);
        Task<RentalAgreementDto?> TerminateRentalAsync(Guid rentalAgreementId);
    }
}
