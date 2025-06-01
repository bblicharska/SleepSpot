using Microsoft.AspNetCore.Mvc;
using ReviewService.Application.Dto;
using ReviewService.Application.Services;

namespace ReviewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ReviewDto>> GetById(Guid id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            return Ok(review);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] ReviewCreateDto dto)
        {
            var id = await _reviewService.CreateReviewAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched review ID");

            await _reviewService.UpdateReviewAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }

        [HttpGet("user-reviewed/{reviewedId:guid}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetForUser(Guid reviewedId)
        {
            var reviews = await _reviewService.GetReviewsForUserAsync(reviewedId);
            return Ok(reviews);
        }

        [HttpGet("user-reviewer/{reviewerId:guid}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByUser(Guid reviewerId)
        {
            var reviews = await _reviewService.GetReviewsByUserAsync(reviewerId);
            return Ok(reviews);
        }

        [HttpGet("property/{propertyId:guid}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetForProperty(Guid propertyId)
        {
            var reviews = await _reviewService.GetReviewsForPropertyAsync(propertyId);
            return Ok(reviews);
        }

        [HttpGet("landlord/{landlordId:guid}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetLandlordReviews(Guid landlordId)
        {
            var reviews = await _reviewService.GetLandlordReviewsAsync(landlordId);
            return Ok(reviews);
        }

        [HttpGet("tenant/{tenantId:guid}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetTenantReviews(Guid tenantId)
        {
            var reviews = await _reviewService.GetTenantReviewsAsync(tenantId);
            return Ok(reviews);
        }

        [HttpGet("average-rating/{userId:guid}")]
        public async Task<ActionResult<double>> GetAverageRating(Guid userId)
        {
            var average = await _reviewService.GetUserAverageRatingAsync(userId);
            return Ok(average);
        }

        [HttpGet("exists")]
        public async Task<ActionResult<bool>> HasUserReviewed(
            [FromQuery] Guid reviewerId,
            [FromQuery] Guid reviewedId,
            [FromQuery] Guid? propertyId = null)
        {
            var exists = await _reviewService.HasUserReviewedAsync(reviewerId, reviewedId, propertyId);
            return Ok(exists);
        }
    }
}
