using ReviewService.Domain.Contracts;
using ReviewService.Domain.Models;
using ReviewService.Application.Dto;
using AutoMapper;
using ReviewService.Application.Interfaces;

namespace ReviewService.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserClient _userClient;

        public ReviewService(
            IReviewUnitOfWork unitOfWork,
            IMapper mapper,
            IUserClient userClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClient = userClient;
        }

        public async Task<ReviewDto> GetReviewByIdAsync(Guid id)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Review with ID {id} not found.");
            var reviewDto = _mapper.Map<ReviewDto>(review);

            reviewDto.Reviewer = await _userClient.GetUserByIdAsync(reviewDto.ReviewerId);
            reviewDto.Reviewed = await _userClient.GetUserByIdAsync(reviewDto.ReviewedId);

            return reviewDto;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync();
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            await EnrichReviewsWithUsersAsync(reviewDtos);
            return reviewDtos;
        }


        public async Task<Guid> CreateReviewAsync(ReviewCreateDto dto)
        {
            var review = _mapper.Map<Review>(dto);
            review.Id = Guid.NewGuid();
            review.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.ReviewRepository.AddAsync(review);
            return review.Id;
        }

        public async Task UpdateReviewAsync(ReviewUpdateDto dto)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(dto.Id)
                         ?? throw new KeyNotFoundException($"Review with ID {dto.Id} not found.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            await _unitOfWork.ReviewRepository.UpdateAsync(review);
        }

        public async Task DeleteReviewAsync(Guid id)
        {
            await _unitOfWork.ReviewRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsForUserAsync(Guid reviewedId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByReviewedIdAsync(reviewedId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            await EnrichReviewsWithUsersAsync(reviewDtos);
            return reviewDtos;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(Guid reviewerId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByReviewerIdAsync(reviewerId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            await EnrichReviewsWithUsersAsync(reviewDtos);
            return reviewDtos;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsForPropertyAsync(Guid propertyId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByPropertyIdAsync(propertyId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            await EnrichReviewsWithUsersAsync(reviewDtos);
            return reviewDtos;
        }

        public async Task<IEnumerable<ReviewDto>> GetLandlordReviewsAsync(Guid landlordId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByReviewedIdAsync(landlordId);
            var filteredReviews = reviews.Where(r => r.ReviewedRole == "Landlord");
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(filteredReviews);
            await EnrichReviewsWithUsersAsync(reviewDtos);
            return reviewDtos;
        }

        public async Task<IEnumerable<ReviewDto>> GetTenantReviewsAsync(Guid tenantId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByReviewedIdAsync(tenantId);
            var filteredReviews = reviews.Where(r => r.ReviewedRole == "Tenant");
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(filteredReviews);
            await EnrichReviewsWithUsersAsync(reviewDtos);
            return reviewDtos;
        }

        public async Task<double> GetUserAverageRatingAsync(Guid userId)
        {
            return await _unitOfWork.ReviewRepository.GetAverageRatingByReviewedIdAsync(userId);
        }

        public async Task<bool> HasUserReviewedAsync(Guid reviewerId, Guid reviewedId, Guid? propertyId)
        {
            return await _unitOfWork.ReviewRepository.ExistsAsync(reviewerId, reviewedId, propertyId);
        }

        private async Task EnrichReviewsWithUsersAsync(IEnumerable<ReviewDto> reviewDtos)
        {
            foreach (var reviewDto in reviewDtos)
            {
                reviewDto.Reviewer = await _userClient.GetUserByIdAsync(reviewDto.ReviewerId);
                reviewDto.Reviewed = await _userClient.GetUserByIdAsync(reviewDto.ReviewedId);
            }
        }
    }

}
