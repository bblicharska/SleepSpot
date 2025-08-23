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
        Task<Review> GetByIdAsync(Guid id);
        Task<IEnumerable<Review>> GetAllAsync();
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Review>> GetByPropertyIdAsync(Guid propertyId);
        Task<IEnumerable<Review>> GetByRoomIdAsync(Guid roomId);
        Task<IEnumerable<Review>> GetByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<Review>> GetByReviewerIdAsync(Guid reviewerId);
        Task<double> GetAverageRatingByPropertyIdAsync(Guid propertyId);
        Task<double> GetAverageRatingByRoomIdAsync(Guid roomId);
        Task<bool> ExistsAsync(Guid reviewerId, Guid? propertyId, Guid? roomId);
    }
}
