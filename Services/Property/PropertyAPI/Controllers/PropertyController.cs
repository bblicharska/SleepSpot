using Microsoft.AspNetCore.Mvc;
using PropertyService.Application.Dto;
using PropertyService.Application.Services;
using PropertyService.Domain.Enums;
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

            [HttpGet]
            public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAllProperties(
     [FromQuery] PropertySortBy sortBy = PropertySortBy.CreatedAt,
     [FromQuery] SortDirection sortDirection = SortDirection.Descending)
            {
                try
                {
                    var properties = await _propertyService.GetAllPropertiesAsync(sortBy, sortDirection);
                    return Ok(properties);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving all properties");
                    return StatusCode(500, "An unexpected error occurred while retrieving properties.");
                }
            }

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
            public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsForProperty(
    Guid propertyId,
    [FromQuery] RoomSortBy sortBy = RoomSortBy.CreatedAt,
    [FromQuery] SortDirection sortDirection = SortDirection.Descending)
            {
                try
                {
                    var rooms = await _propertyService.GetRoomsForPropertyAsync(propertyId, sortBy, sortDirection);
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

            [HttpDelete("rooms/{roomId}")]
            public async Task<IActionResult> DeleteRoom(Guid roomId)
            {
                try
                {
                    await _propertyService.DeleteRoomAsync(roomId);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting room with ID {RoomId}", roomId);
                    return StatusCode(500, "An unexpected error occurred while deleting the room.");
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

            [HttpGet("search")]
            public async Task<ActionResult<IEnumerable<PropertyDto>>> SearchProperties([FromQuery] PropertyFilterDto filters)
            {
                try
                {
                    var result = await _propertyService.SearchPropertiesAsync(filters);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error searching properties");
                    return StatusCode(500, "An unexpected error occurred while searching properties.");
                }
            }

            [HttpGet("rooms/search")]
            public async Task<ActionResult<IEnumerable<RoomFilterDto>>> SearchAllRooms([FromQuery] RoomSearchFilterDto filters)
            {
                try
                {
                    var rooms = await _propertyService.SearchAllRoomsAsync(filters);
                    return Ok(rooms);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error searching all rooms");
                    return StatusCode(500, "An unexpected error occurred while searching rooms.");
                }
            }

            [HttpGet("rooms/{roomId}")]
            public async Task<ActionResult<RoomDto>> GetRoomById(Guid roomId)
            {
                try
                {
                    var room = await _propertyService.GetRoomByIdAsync(roomId);
                    return Ok(room);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving room with ID {RoomId}", roomId);
                    return StatusCode(500, "An unexpected error occurred while retrieving the room.");
                }
            }

            [HttpPost("rooms/{roomId}/images")]
            public async Task<IActionResult> UploadRoomImages(Guid roomId, [FromForm] List<IFormFile> files)
            {
                if (files == null || !files.Any())
                    return BadRequest("No files provided");

                try
                {
                    var uploadedImages = await _propertyService.AddMultipleRoomImagesAsync(roomId, files);

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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading images for room {RoomId}", roomId);
                    return StatusCode(500, "An unexpected error occurred while uploading room images.");
                }
            }

            [HttpGet("rooms/{roomId}/details")]
            public async Task<IActionResult> GetRoomWithPropertyDetails(Guid roomId)
            {
                try
                {
                    var roomDetails = await _propertyService.GetRoomWithPropertyDetailsAsync(roomId);

                    if (roomDetails == null)
                    {
                        return NotFound($"Room with ID {roomId} not found.");
                    }

                    return Ok(roomDetails);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching room details for room ID: {RoomId}", roomId);
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }

            [HttpPut("rooms/{roomId}/availability")]
            public async Task<IActionResult> UpdateRoomAvailability(Guid roomId, [FromBody] AvailabilityUpdateDto dto)
            {
                if (dto == null) return BadRequest("Payload is required.");
                try
                {
                    await _propertyService.UpdateRoomAvailabilityAsync(roomId, dto.IsAvailable, dto.AvailableSince);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogWarning(ex, "Room not found when updating availability: {RoomId}", roomId);
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating availability for room {RoomId}", roomId);
                    return StatusCode(500, "An unexpected error occurred while updating room availability.");
                }
            }

            [HttpPut("{propertyId}/availability")]
            public async Task<IActionResult> UpdatePropertyAvailability(Guid propertyId, [FromBody] AvailabilityUpdateDto dto)
            {
                if (dto == null) return BadRequest("Payload is required.");
                try
                {
                    await _propertyService.UpdatePropertyAvailabilityAsync(propertyId, dto.IsAvailable, dto.AvailableSince);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogWarning(ex, "Property not found when updating availability: {PropertyId}", propertyId);
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating availability for property {PropertyId}", propertyId);
                    return StatusCode(500, "An unexpected error occurred while updating property availability.");
                }
            }
        }

    }
}