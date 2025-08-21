using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Contracts;
using RentalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Infrastructure.Repositories
{
    public class RentalAgreementRepository : IRentalAgreementRepository
    {
        private readonly RentalDbContext _context;

        public RentalAgreementRepository(RentalDbContext context)
        {
            _context = context;
        }

        public async Task<RentalAgreement?> GetByIdAsync(Guid id)
        {
            return await _context.RentalAgreements.FindAsync(id);
        }

        public async Task<IEnumerable<RentalAgreement>> GetAllAsync()
        {
            return await _context.RentalAgreements.ToListAsync();
        }

        public async Task<IEnumerable<RentalAgreement>> GetByUserIdAsync(Guid userId)
        {
            return await _context.RentalAgreements
                .Where(ra => ra.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalAgreement>> GetByGroupIdAsync(Guid groupId)
        {
            return await _context.RentalAgreements
                .Where(ra => ra.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalAgreement>> GetByPropertyIdAsync(Guid propertyId)
        {
            return await _context.RentalAgreements
                .Where(ra => ra.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalAgreement>> GetActiveAgreementsAsync()
        {
            return await _context.RentalAgreements
                .Where(ra => ra.Status == RentalAgreementStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalAgreement>> GetActiveByUserIdAsync(Guid userId)
        {
            return await _context.RentalAgreements
                .Where(ra => ra.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalAgreement>> GetActiveByGroupIdAsync(Guid groupId)
        {
            return await _context.RentalAgreements
                .Where(ra => ra.GroupId == groupId)
                .ToListAsync();
        }

        public async Task AddAsync(RentalAgreement agreement)
        {
            await _context.RentalAgreements.AddAsync(agreement);
        }

        public void Update(RentalAgreement agreement)
        {
            _context.RentalAgreements.Update(agreement);
        }

        public void Remove(RentalAgreement agreement)
        {
            _context.RentalAgreements.Remove(agreement);
        }
    }
}
