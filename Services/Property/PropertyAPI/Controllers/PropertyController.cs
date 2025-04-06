using Microsoft.AspNetCore.Mvc;
using PropertyService.Application.Dto;
using PropertyService.Application.Services;

namespace PropertyAPI.Controllers
{
    namespace PropertyService.Api.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PropertyController : ControllerBase
        {
            private readonly IPropertyService _propertyService;

            // Dependency Injection wstrzykująca IPropertyService
            public PropertyController(IPropertyService propertyService)
            {
                _propertyService = propertyService;
            }

            // GET api/property
            [HttpGet]
            public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAllProperties()
            {
                var properties = await _propertyService.GetAllPropertiesAsync();
                return Ok(properties); // Zwraca status 200 oraz listę nieruchomości
            }

            // GET api/property/{id}
            [HttpGet("{id}")]
            public async Task<ActionResult<PropertyDto>> GetPropertyById(Guid id)
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);

                if (property == null)
                {
                    return NotFound(); // Zwraca 404, jeśli nieruchomość o takim id nie istnieje
                }

                return Ok(property); // Zwraca status 200 oraz dane nieruchomości
            }

            // POST api/property
            [HttpPost]
            public async Task<ActionResult<PropertyDto>> CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
            {
                if (createPropertyDto == null)
                {
                    return BadRequest(); // Zwraca 400, jeśli dane są niepoprawne
                }

                var property = await _propertyService.CreatePropertyAsync(createPropertyDto);

                // Zwraca status 201 oraz dane utworzonej nieruchomości
                return CreatedAtAction(nameof(GetPropertyById), new { id = property.Id }, property);
            }

            // PUT api/property/{id}
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyDto updatePropertyDto)
            {
                if (updatePropertyDto == null)
                {
                    return BadRequest("Invalid request data.");
                }

                try
                {
                    var updatedProperty = await _propertyService.UpdatePropertyAsync(id, updatePropertyDto);
                    return NoContent(); // 204 - Brak treści, ale operacja zakończona sukcesem
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] {ex.Message}");
                    return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
                }
            }

            // DELETE api/property/{id}
            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteProperty(Guid id)
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);
                if (property == null)
                {
                    return NotFound(); // Zwraca 404, jeśli nieruchomość o takim id nie istnieje
                }

                await _propertyService.DeletePropertyAsync(id);
                return NoContent(); // Zwraca 204, jeśli usunięcie przebiegło pomyślnie
            }
        }
    }
}