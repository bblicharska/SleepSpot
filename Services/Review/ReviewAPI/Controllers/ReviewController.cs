using Microsoft.AspNetCore.Mvc;
using ReviewService.Application.Dto;
using ReviewService.Application.Services;
using Shared.Dto;

namespace ReviewAPI.Controllers
{
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

            [HttpGet("property/{propertyId:guid}")]
            public async Task<ActionResult<IEnumerable<ReviewDto>>> GetForProperty(Guid propertyId)
            {
                var reviews = await _reviewService.GetReviewsForPropertyAsync(propertyId);
                return Ok(reviews);
            }

            [HttpGet("room/{roomId:guid}")]
            public async Task<ActionResult<IEnumerable<ReviewDto>>> GetForRoom(Guid roomId)
            {
                var reviews = await _reviewService.GetReviewsForRoomAsync(roomId);
                return Ok(reviews);
            }

            [HttpGet("owner/{ownerId:guid}")]
            public async Task<ActionResult<IEnumerable<ReviewDto>>> GetForOwner(Guid ownerId)
            {
                var reviews = await _reviewService.GetReviewsForOwnerAsync(ownerId);
                return Ok(reviews);
            }
        }
    }

}
