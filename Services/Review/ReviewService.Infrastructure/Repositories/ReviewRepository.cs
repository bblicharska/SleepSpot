using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReviewService.Domain.Contracts;
using ReviewService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDbContext _context;
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(ReviewDbContext context, ILogger<ReviewRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Review> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Reviews
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving review with ID {ReviewId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            try
            {
                return await _context.Reviews
                    .AsNoTracking()
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all reviews");
                throw;
            }
        }

        public async Task AddAsync(Review review)
        {
            try
            {
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();

                _context.Entry(review).State = EntityState.Detached; // Clear tracking
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                throw;
            }
        }

        public async Task UpdateAsync(Review review)
        {
            try
            {
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();

                _context.Entry(review).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review with ID {ReviewId}", review.Id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review != null)
                {
                    _context.Reviews.Remove(review);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review with ID {ReviewId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetByReviewedIdAsync(Guid reviewedId)
        {
            try
            {
                return await _context.Reviews
                    .AsNoTracking()
                    .Where(r => r.ReviewedId == reviewedId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews for user ID {ReviewedId}", reviewedId);
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetByReviewerIdAsync(Guid reviewerId)
        {
            try
            {
                return await _context.Reviews
                    .AsNoTracking()
                    .Where(r => r.ReviewerId == reviewerId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews by reviewer ID {ReviewerId}", reviewerId);
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetByPropertyIdAsync(Guid propertyId)
        {
            try
            {
                return await _context.Reviews
                    .AsNoTracking()
                    .Where(r => r.PropertyId == propertyId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews for property ID {PropertyId}", propertyId);
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetByReviewedRoleAsync(string role)
        {
            try
            {
                return await _context.Reviews
                    .AsNoTracking()
                    .Where(r => r.ReviewedRole == role)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews for role {Role}", role);
                throw;
            }
        }

        public async Task<double> GetAverageRatingByReviewedIdAsync(Guid reviewedId)
        {
            try
            {
                return await _context.Reviews
                    .Where(r => r.ReviewedId == reviewedId)
                    .AverageAsync(r => r.Rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average rating for user ID {ReviewedId}", reviewedId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Guid reviewerId, Guid reviewedId, Guid? propertyId)
        {
            try
            {
                return await _context.Reviews
                    .AnyAsync(r => r.ReviewerId == reviewerId
                                && r.ReviewedId == reviewedId
                                && (propertyId == null || r.PropertyId == propertyId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if review exists");
                throw;
            }
        }
    }
}
