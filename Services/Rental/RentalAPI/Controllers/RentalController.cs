using Microsoft.AspNetCore.Mvc;
using RentalService.Application.Dto;
using RentalService.Application.Services;

namespace RentalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentalAgreementDto>>> GetAll()
        {
            var agreements = await _rentalService.GetAllAsync();
            return Ok(agreements);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RentalAgreementDto>> GetById(Guid id)
        {
            var agreement = await _rentalService.GetByIdAsync(id);
            if (agreement == null)
                return NotFound();

            return Ok(agreement);
        }

        [HttpPost]
        public async Task<ActionResult<RentalAgreementDto>> Create([FromBody] CreateRentalAgreementDto dto)
        {
            var created = await _rentalService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RentalAgreementDto>> Update(Guid id, [FromBody] UpdateRentalAgreementDto dto)
        {
            var updated = await _rentalService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _rentalService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<IEnumerable<RentalAgreementDto>>> GetActiveByUserId(Guid userId)
        {
            var agreements = await _rentalService.GetActiveByUserIdAsync(userId);
            return Ok(agreements);
        }
    }
}
