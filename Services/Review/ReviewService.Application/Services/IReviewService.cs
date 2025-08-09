using ReviewService.Application.Dto;
using ReviewService.Domain.Models;
using Shared.Dto;
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

        Task<IEnumerable<ReviewDto>> GetReviewsForPropertyAsync(Guid propertyId);
        Task<IEnumerable<ReviewDto>> GetReviewsForRoomAsync(Guid roomId);
        Task<IEnumerable<ReviewDto>> GetReviewsForOwnerAsync(Guid ownerId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(Guid reviewerId);

        Task<double> GetAverageRatingForPropertyAsync(Guid propertyId);
        Task<double> GetAverageRatingForRoomAsync(Guid roomId);
    }

}
