using ReviewService.Domain.Contracts;
using ReviewService.Domain.Models;
using ReviewService.Application.Dto;
using AutoMapper;
using Shared.Dto;

namespace ReviewService.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewDto> GetReviewByIdAsync(Guid id)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Review with ID {id} not found.");
            var reviewDto = _mapper.Map<ReviewDto>(review);

            return reviewDto;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync();
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
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

        public async Task<IEnumerable<ReviewDto>> GetReviewsForPropertyAsync(Guid propertyId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByPropertyIdAsync(propertyId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return reviewDtos;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsForRoomAsync(Guid roomId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByRoomIdAsync(roomId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return reviewDtos;
        }
        
        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(Guid reviewerId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByReviewerIdAsync(reviewerId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return reviewDtos;
        }

        public async Task<double> GetAverageRatingForPropertyAsync(Guid propertyId)
        {
            return await _unitOfWork.ReviewRepository.GetAverageRatingByPropertyIdAsync(propertyId);
        }

        public async Task<double> GetAverageRatingForRoomAsync(Guid roomId)
        {
            return await _unitOfWork.ReviewRepository.GetAverageRatingByRoomIdAsync(roomId);
        }
    }

}
