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

        // Opinie o property
        Task<IEnumerable<ReviewDto>> GetReviewsForPropertyAsync(Guid propertyId);

        // Opinie o pokoju (jeśli chcesz)
        Task<IEnumerable<ReviewDto>> GetReviewsForRoomAsync(Guid roomId);

        // Opinie napisane przez użytkownika (recenzenta)
        Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(Guid reviewerId);

        // Możesz też mieć średnią ocenę dla property i pokoju
        Task<double> GetAverageRatingForPropertyAsync(Guid propertyId);
        Task<double> GetAverageRatingForRoomAsync(Guid roomId);
    }

}
