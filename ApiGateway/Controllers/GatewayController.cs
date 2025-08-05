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

        [HttpGet("group-listing-details/{listingId:guid}")]
        public async Task<IActionResult> GetGroupListingDetails(Guid listingId)
        {
            var groupClient = _httpClientFactory.CreateClient("GroupClient");
            var propertyClient = _httpClientFactory.CreateClient("PropertyClient");
            var userClient = _httpClientFactory.CreateClient("UserClient");

            try
            {
                _logger.LogInformation("Fetching group listing details for {ListingId}", listingId);

                // 1. Fetch the group listing with applications
                _logger.LogInformation("Fetching listing from: /api/Group/listings/{ListingId}", listingId);
                var listingResponse = await groupClient.GetAsync($"/api/Group/listings/{listingId}");

                if (!listingResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Group listing {ListingId} not found. Status: {StatusCode}", listingId, listingResponse.StatusCode);
                    return NotFound($"Group listing {listingId} not found. Status: {listingResponse.StatusCode}");
                }

                var listing = await listingResponse.Content.ReadFromJsonAsync<GroupListingDto>();
                if (listing == null)
                {
                    _logger.LogWarning("Group listing {ListingId} returned null", listingId);
                    return NotFound($"Group listing {listingId} returned null.");
                }

                _logger.LogInformation("Successfully fetched listing: {Title}", listing.Title);

                if (listing.PropertyId.HasValue)
                {
                    if (listing.Property != null)
                    {
                        _logger.LogInformation("Property details already included in response for PropertyId: {PropertyId}", listing.PropertyId.Value);
                    }
                    else
                    {
                        _logger.LogInformation("Fetching property details for PropertyId: {PropertyId}", listing.PropertyId.Value);
                        var propertyResponse = await propertyClient.GetAsync($"/api/Property/{listing.PropertyId.Value}");
                        if (propertyResponse.IsSuccessStatusCode)
                        {
                            var property = await propertyResponse.Content.ReadFromJsonAsync<PropertyDto>();
                            listing.Property = property;
                            _logger.LogInformation("Successfully fetched property details");
                        }
                    }
                }

                if (listing.RoomId.HasValue)
                {
                    _logger.LogInformation("Fetching room details for RoomId: {RoomId}", listing.RoomId.Value);

                    var roomResponse = await propertyClient.GetAsync($"/api/Property/rooms/{listing.RoomId.Value}/details");
                    if (roomResponse.IsSuccessStatusCode)
                    {
                        var room = await roomResponse.Content.ReadFromJsonAsync<RoomWithPropertyDetailsDto>();
                        listing.Room = room;
                        _logger.LogInformation("Successfully fetched room details");
                    }
                }

                // 4. Fetch group details and members (only if group is not already included or is null)
                if (listing.GroupId != Guid.Empty && listing.Group == null)
                {
                    _logger.LogInformation("Group not included in response, fetching group details for GroupId: {GroupId}", listing.GroupId);
                    var groupResponse = await groupClient.GetAsync($"/api/Group/{listing.GroupId}");
                    if (groupResponse.IsSuccessStatusCode)
                    {
                        var group = await groupResponse.Content.ReadFromJsonAsync<GroupDto>();
                        listing.Group = group;
                        _logger.LogInformation("Successfully fetched group details: {GroupName}", group?.Name);

                        // Fetch detailed user info for group members if needed
                        if (group?.Members?.Any() == true)
                        {
                            await EnrichGroupMembersWithUserDetails(group, userClient);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to fetch group {GroupId}. Status: {StatusCode}", listing.GroupId, groupResponse.StatusCode);
                    }
                }
                else if (listing.Group?.Members?.Any() == true)
                {
                    // Group is already included, just enrich members with user details
                    _logger.LogInformation("Group already included, enriching member details");
                    await EnrichGroupMembersWithUserDetails(listing.Group, userClient);
                }

                // 5. Fetch applicant details for applications
                if (listing.Applications?.Any() == true)
                {
                    _logger.LogInformation("Fetching applicant details for {ApplicationCount} applications", listing.Applications.Count);
                    await EnrichApplicationsWithUserDetails(listing.Applications, userClient);
                }

                _logger.LogInformation("Successfully completed fetching all details for listing {ListingId}", listingId);
                return Ok(listing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching group listing details for {ListingId}", listingId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task EnrichGroupMembersWithUserDetails(GroupDto group, HttpClient userClient)
        {
            _logger.LogInformation("Fetching user details for {MemberCount} group members", group.Members.Count);
            var memberTasks = group.Members.Select(async member =>
            {
                try
                {
                    var userResponse = await userClient.GetAsync($"/api/User/{member.UserId}");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var userDetails = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                        if (userDetails != null)
                        {
                            member.User = userDetails;
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to fetch user {UserId}. Status: {StatusCode}", member.UserId, userResponse.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching user details for {UserId}", member.UserId);
                }
                return member;
            });

            await Task.WhenAll(memberTasks);
        }

        private async Task EnrichApplicationsWithUserDetails(List<RoomApplicationDto> applications, HttpClient userClient)
        {
            var applicantIds = applications.Select(a => a.ApplicantUserId).Distinct();
            var applicantTasks = applicantIds.Select(async id =>
            {
                try
                {
                    var userResponse = await userClient.GetAsync($"/api/User/{id}");
                    return userResponse.IsSuccessStatusCode
                        ? await userResponse.Content.ReadFromJsonAsync<UserDto>()
                        : null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching applicant user details for {UserId}", id);
                    return null;
                }
            });

            var applicants = await Task.WhenAll(applicantTasks);
            foreach (var application in applications)
            {
                application.Applicant = applicants.FirstOrDefault(u => u?.Id == application.ApplicantUserId);
            }
        }
    }
}