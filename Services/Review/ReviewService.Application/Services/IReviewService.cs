using ReviewService.Application.Dto;
using ReviewService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Application.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> GetReviewByIdAsync(Guid id);
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();

        Task<Guid> CreateReviewAsync(ReviewCreateDto dto);
        Task UpdateReviewAsync(ReviewUpdateDto dto);
        Task DeleteReviewAsync(Guid id);

        Task<IEnumerable<ReviewDto>> GetReviewsForUserAsync(Guid reviewedId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(Guid reviewerId);
        Task<IEnumerable<ReviewDto>> GetReviewsForPropertyAsync(Guid propertyId);

        Task<IEnumerable<ReviewDto>> GetLandlordReviewsAsync(Guid landlordId);
        Task<IEnumerable<ReviewDto>> GetTenantReviewsAsync(Guid tenantId);

        Task<double> GetUserAverageRatingAsync(Guid userId);
        Task<bool> HasUserReviewedAsync(Guid reviewerId, Guid reviewedId, Guid? propertyId);
    }
}
