using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using System.Net.Http.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("gateway")]
    public class GatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GatewayController> _logger;

        public GatewayController(IHttpClientFactory httpClientFactory, ILogger<GatewayController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("property-details/{propertyId:guid}")]
        public async Task<IActionResult> GetPropertyDetails(Guid propertyId)
        {
            var propertyClient = _httpClientFactory.CreateClient("PropertyClient");
            var reviewClient = _httpClientFactory.CreateClient("ReviewClient");
            var userClient = _httpClientFactory.CreateClient("UserClient");

            try
            {
                // Fix: Use the correct property API endpoint
                var propertyResponse = await propertyClient.GetAsync($"/api/Property/{propertyId}");
                if (!propertyResponse.IsSuccessStatusCode)
                    return NotFound($"Property {propertyId} not found.");

                var property = await propertyResponse.Content.ReadFromJsonAsync<PropertyDto>();
                if (property == null)
                    return NotFound($"Property {propertyId} returned null.");

                // Fetch property reviews
                var reviewsResponse = await reviewClient.GetAsync($"/api/Review/property/{propertyId}");

                if (reviewsResponse.IsSuccessStatusCode)
                {
                    var reviews = await reviewsResponse.Content.ReadFromJsonAsync<List<ReviewDto>>();
                    if (reviews?.Any() == true)
                    {
                        var reviewerIds = reviews.Select(r => r.ReviewerId).Distinct();
                        var userTasks = reviewerIds.Select(async id =>
                        {
                            var userResponse = await userClient.GetAsync($"/api/User/{id}");
                            return userResponse.IsSuccessStatusCode
                                ? await userResponse.Content.ReadFromJsonAsync<UserDto>()
                                : null;
                        });

                        var users = await Task.WhenAll(userTasks);
                        foreach (var review in reviews)
                        {
                            review.Reviewer = users.FirstOrDefault(u => u?.Id == review.ReviewerId);
                        }

                        property.Reviews = reviews;
                    }
                }

                // Fetch owner
                var ownerResponse = await userClient.GetAsync($"/api/User/{property.OwnerId}");
                if (ownerResponse.IsSuccessStatusCode)
                {
                    var owner = await ownerResponse.Content.ReadFromJsonAsync<UserDto>();
                    property.Owner = owner ?? new UserDto();
                }

                return Ok(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching property details for {PropertyId}", propertyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("room-details/{roomId:guid}")]
        public async Task<IActionResult> GetRoomDetails(Guid roomId)
        {
            var propertyClient = _httpClientFactory.CreateClient("PropertyClient");
            var reviewClient = _httpClientFactory.CreateClient("ReviewClient");
            var userClient = _httpClientFactory.CreateClient("UserClient");

            try
            {
                // 1. Pobierz dane pokoju i property w jednym wywołaniu
                var roomDetailsResponse = await propertyClient.GetAsync($"/api/Property/rooms/{roomId}/details");
                if (!roomDetailsResponse.IsSuccessStatusCode)
                    return NotFound($"Room {roomId} not found.");

                var roomDetails = await roomDetailsResponse.Content.ReadFromJsonAsync<RoomWithPropertyDetailsDto>();
                if (roomDetails == null)
                    return NotFound($"Room {roomId} returned null.");

                // 2. Pobierz dane właściciela (jeśli OwnerId istnieje)
                if (roomDetails.OwnerId.HasValue)
                {
                    var ownerResponse = await userClient.GetAsync($"/api/User/{roomDetails.OwnerId.Value}");
                    if (ownerResponse.IsSuccessStatusCode)
                    {
                        var owner = await ownerResponse.Content.ReadFromJsonAsync<UserDto>();
                        if (owner != null)
                        {
                            roomDetails.Owner = new OwnerInfoDto
                            {
                                FirstName = owner.FirstName,
                                LastName = owner.LastName,
                                Email = owner.Email
                            };
                        }
                    }
                }

                // 3. Pobierz opinie dla pokoju
                var reviewsResponse = await reviewClient.GetAsync($"/api/Review/room/{roomId}");
                if (reviewsResponse.IsSuccessStatusCode)
                {
                    var reviews = await reviewsResponse.Content.ReadFromJsonAsync<List<ReviewDto>>();
                    if (reviews?.Any() == true)
                    {
                        // Pobierz dane recenzentów
                        var reviewerIds = reviews.Select(r => r.ReviewerId).Distinct();
                        var userTasks = reviewerIds.Select(async id =>
                        {
                            var userResponse = await userClient.GetAsync($"/api/User/{id}");
                            return userResponse.IsSuccessStatusCode
                                ? await userResponse.Content.ReadFromJsonAsync<UserDto>()
                                : null;
                        });

                        var users = await Task.WhenAll(userTasks);
                        foreach (var review in reviews)
                        {
                            review.Reviewer = users.FirstOrDefault(u => u?.Id == review.ReviewerId);
                        }

                        roomDetails.Reviews = reviews;
                    }
                }

                return Ok(roomDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching room details for {RoomId}", roomId);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}