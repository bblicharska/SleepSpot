using ReviewService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Domain.Contracts
{
    public interface IReviewRepository
    {
        // Podstawowe operacje CRUD
        Task<Review> GetByIdAsync(Guid id);
        Task<IEnumerable<Review>> GetAllAsync();
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Guid id);

        // Zapytania specyficzne dla domeny
        Task<IEnumerable<Review>> GetByReviewedIdAsync(Guid reviewedId);
        Task<IEnumerable<Review>> GetByReviewerIdAsync(Guid reviewerId);
        Task<IEnumerable<Review>> GetByPropertyIdAsync(Guid propertyId);
        Task<IEnumerable<Review>> GetByReviewedRoleAsync(string role);
        Task<double> GetAverageRatingByReviewedIdAsync(Guid reviewedId);
        Task<bool> ExistsAsync(Guid reviewerId, Guid reviewedId, Guid? propertyId);
    }
}
