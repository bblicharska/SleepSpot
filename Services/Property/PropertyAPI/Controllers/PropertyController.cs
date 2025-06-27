using Microsoft.AspNetCore.Mvc;
using PropertyService.Application.Dto;
using PropertyService.Application.Services;
using Shared.Dto;

namespace PropertyAPI.Controllers
{
    namespace PropertyService.Api.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PropertyController : ControllerBase
        {
            private readonly IPropertyService _propertyService;
            private readonly ILogger<PropertyController> _logger;

            public PropertyController(IPropertyService propertyService, ILogger<PropertyController> logger)
            {
                _propertyService = propertyService;
                _logger = logger;
            }

            // GET api/property
            [HttpGet]
            public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAllProperties()
            {
                try
                {
                    var properties = await _propertyService.GetAllPropertiesAsync();
                    return Ok(properties);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving all properties");
                    return StatusCode(500, "An unexpected error occurred while retrieving properties.");
                }
            }

            // GET api/property/{id}
            [HttpGet("{id}")]
            public async Task<ActionResult<PropertyDto>> GetPropertyById(Guid id)
            {
                try
                {
                    var property = await _propertyService.GetPropertyByIdAsync(id);
                    return Ok(property);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving property with ID {PropertyId}", id);
                    return StatusCode(500, "An unexpected error occurred while retrieving the property.");
                }
            }

            // POST api/property
            [HttpPost]
            public async Task<ActionResult<PropertyDto>> CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
            {
                if (createPropertyDto == null)
                {
                    return BadRequest("Property data is required.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    var property = await _propertyService.CreatePropertyAsync(createPropertyDto);
                    return CreatedAtAction(nameof(GetPropertyById), new { id = property.Id }, property);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating property");
                    return StatusCode(500, "An unexpected error occurred while creating the property.");
                }
            }

            // PUT api/property/{id}
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyDto updatePropertyDto)
            {
                if (updatePropertyDto == null)
                {
                    return BadRequest("Property data is required.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    var updatedProperty = await _propertyService.UpdatePropertyAsync(id, updatePropertyDto);
                    return Ok(updatedProperty); // Return the updated property instead of NoContent
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating property with ID {PropertyId}", id);
                    return StatusCode(500, "An unexpected error occurred while updating the property.");
                }
            }

            // DELETE api/property/{id}
            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteProperty(Guid id)
            {
                try
                {
                    await _propertyService.DeletePropertyAsync(id);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting property with ID {PropertyId}", id);
                    return StatusCode(500, "An unexpected error occurred while deleting the property.");
                }
            }

            [HttpGet("{propertyId}/rooms")]
            public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsForProperty(Guid propertyId)
            {
                try
                {
                    var rooms = await _propertyService.GetRoomsForPropertyAsync(propertyId);
                    return Ok(rooms);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving rooms for property {PropertyId}", propertyId);
                    return StatusCode(500, "An unexpected error occurred while retrieving rooms.");
                }
            }

            [HttpPost("{propertyId}/rooms")]
            public async Task<ActionResult> AddRoomToProperty(Guid propertyId, [FromBody] CreateRoomDto createRoomDto)
            {
                if (createRoomDto == null)
                {
                    return BadRequest("Room data is required.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    await _propertyService.AddRoomToPropertyAsync(propertyId, createRoomDto);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding room to property {PropertyId}", propertyId);
                    return StatusCode(500, "An unexpected error occurred while adding the room.");
                }
            }

            [HttpPost("rooms/{roomId}/rent")]
            public async Task<ActionResult> RentRoom(Guid roomId)
            {
                try
                {
                    await _propertyService.RentRoomAsync(roomId);
                    return Ok(new { message = "Room rented successfully" });
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error renting room {RoomId}", roomId);
                    return StatusCode(500, "An unexpected error occurred while renting the room.");
                }
            }

            [HttpPost("{id}/images")]
            public async Task<IActionResult> UploadImages(Guid id, [FromForm] List<IFormFile> files)
            {
                if (files == null || !files.Any())
                    return BadRequest("No files provided");

                try
                {
                    var uploadedImages = await _propertyService.AddMultipleImagesAsync(id, files);
                    var imageDtos = uploadedImages.Select(img => new PropertyImageDto
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl,
                        OriginalFileName = img.OriginalFileName,
                        IsPrimary = img.IsPrimary,
                        DisplayOrder = img.DisplayOrder
                    }).ToList();

                    return Ok(imageDtos);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading images for property {PropertyId}", id);
                    return StatusCode(500, "An unexpected error occurred while uploading images.");
                }
            }

            [HttpGet("{id}/images")]
            public async Task<IActionResult> GetPropertyImages(Guid id)
            {
                try
                {
                    var images = await _propertyService.GetPropertyImagesAsync(id);
                    var imageDtos = images.Select(img => new PropertyImageDto
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl,
                        OriginalFileName = img.OriginalFileName,
                        IsPrimary = img.IsPrimary,
                        DisplayOrder = img.DisplayOrder
                    }).ToList();

                    return Ok(imageDtos);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving images for property {PropertyId}", id);
                    return StatusCode(500, "An unexpected error occurred while retrieving images.");
                }
            }

            [HttpDelete("images/{imageId}")]
            public async Task<IActionResult> DeleteImage(Guid imageId)
            {
                try
                {
                    var result = await _propertyService.DeleteImageAsync(imageId);
                    return result ? Ok(new { message = "Image deleted successfully" }) : NotFound("Image not found");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting image {ImageId}", imageId);
                    return StatusCode(500, "An unexpected error occurred while deleting the image.");
                }
            }

            [HttpPut("images/{imageId}/primary")]
            public async Task<IActionResult> SetPrimaryImage(Guid imageId)
            {
                try
                {
                    var result = await _propertyService.SetPrimaryImageAsync(imageId);
                    return result ? Ok(new { message = "Primary image set successfully" }) : NotFound("Image not found");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error setting primary image {ImageId}", imageId);
                    return StatusCode(500, "An unexpected error occurred while setting primary image.");
                }
            }

            [HttpGet("{id}/with-images")]
            public async Task<IActionResult> GetPropertyWithImages(Guid id)
            {
                try
                {
                    var property = await _propertyService.GetPropertyByIdAsync(id);
                    var images = await _propertyService.GetPropertyImagesAsync(id);

                    var result = new
                    {
                        Property = property,
                        Images = images.Select(img => new PropertyImageDto
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl,
                            OriginalFileName = img.OriginalFileName,
                            IsPrimary = img.IsPrimary,
                            DisplayOrder = img.DisplayOrder
                        }).ToList()
                    };

                    return Ok(result);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving property with images {PropertyId}", id);
                    return StatusCode(500, "An unexpected error occurred while retrieving property with images.");
                }
            }

            // Add search endpoint that seems to be missing
            [HttpGet("search")]
            public async Task<ActionResult<IEnumerable<PropertyDto>>> SearchProperties(
                [FromQuery] string? location,
                [FromQuery] decimal? minPrice,
                [FromQuery] decimal? maxPrice)
            {
                try
                {
                    var properties = await _propertyService.SearchPropertiesAsync(location, minPrice, maxPrice);
                    return Ok(properties);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error searching properties with location: {Location}, minPrice: {MinPrice}, maxPrice: {MaxPrice}",
                        location, minPrice, maxPrice);
                    return StatusCode(500, "An unexpected error occurred while searching properties.");
                }
            }

            // Add endpoint to get properties by owner
            [HttpGet("owner/{ownerId}")]
            public async Task<ActionResult<IEnumerable<PropertyDto>>> GetPropertiesByOwner(Guid ownerId)
            {
                try
                {
                    var properties = await _propertyService.GetPropertiesByOwnerIdAsync(ownerId);
                    return Ok(properties);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving properties for owner {OwnerId}", ownerId);
                    return StatusCode(500, "An unexpected error occurred while retrieving properties for owner.");
                }
            }
        }

    }
}