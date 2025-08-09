using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Dto;
using System.Net.Http.Json;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("gateway")]
    public class GatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GatewayController> _logger;
        private readonly IDistributedCache _cache;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public GatewayController(
            IHttpClientFactory httpClientFactory,
            ILogger<GatewayController> logger,
            IDistributedCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = cache;
        }

        private async Task<T> SafeCall<T>(Func<Task<T>> call, string serviceName, T defaultValue)
        {
            try
            {
                return await call();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "{Service} unavailable. Returning default value.", serviceName);
                return defaultValue;
            }
        }

        private async Task<T?> GetOrSetCacheAsync<T>(string cacheKey, Func<Task<T>> fetchFunc, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cache key {CacheKey}", cacheKey);
                }
            }

            var freshData = await fetchFunc();

            try
            {
                var serialized = JsonSerializer.Serialize(freshData, _jsonOptions);
                var options = new DistributedCacheEntryOptions();
                if (absoluteExpirationRelativeToNow.HasValue)
                {
                    options.SetAbsoluteExpiration(absoluteExpirationRelativeToNow.Value);
                }
                await _cache.SetStringAsync(cacheKey, serialized, options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to serialize/set cache for key {CacheKey}", cacheKey);
            }

            return freshData;
        }

        [HttpGet("property-details/{propertyId:guid}")]
        public async Task<IActionResult> GetPropertyDetails(Guid propertyId)
        {
            var cacheKey = $"property-details-{propertyId}";
            var propertyClient = _httpClientFactory.CreateClient("PropertyClient");
            var reviewClient = _httpClientFactory.CreateClient("ReviewClient");
            var userClient = _httpClientFactory.CreateClient("UserClient");

            var propertyDetails = await GetOrSetCacheAsync(cacheKey, async () =>
            {
                var propertyResponse = await propertyClient.GetAsync($"/api/Property/{propertyId}");
                if (!propertyResponse.IsSuccessStatusCode) return null;

                var property = await propertyResponse.Content.ReadFromJsonAsync<PropertyDto>();
                if (property == null) return null;

                var reviews = await SafeCall(async () =>
                {
                    var revResp = await reviewClient.GetAsync($"/api/Review/property/{propertyId}");
                    if (!revResp.IsSuccessStatusCode) return new List<ReviewDto>();
                    var revs = await revResp.Content.ReadFromJsonAsync<List<ReviewDto>>();
                    return revs ?? new List<ReviewDto>();
                }, "ReviewService", new List<ReviewDto>());

                if (reviews.Any())
                {
                    var reviewerIds = reviews.Select(r => r.ReviewerId).Distinct();
                    var users = await Task.WhenAll(reviewerIds.Select(async id =>
                    {
                        return await SafeCall(async () =>
                        {
                            var userResp = await userClient.GetAsync($"/api/User/{id}");
                            if (!userResp.IsSuccessStatusCode) return null;
                            return await userResp.Content.ReadFromJsonAsync<UserDto>();
                        }, "UserService", null);
                    }));

                    foreach (var review in reviews)
                    {
                        review.Reviewer = users.FirstOrDefault(u => u?.Id == review.ReviewerId);
                    }
                }
                property.Reviews = reviews;

                var owner = await SafeCall(async () =>
                {
                    var ownerResp = await userClient.GetAsync($"/api/User/{property.OwnerId}");
                    if (!ownerResp.IsSuccessStatusCode) return new UserDto();
                    return await ownerResp.Content.ReadFromJsonAsync<UserDto>() ?? new UserDto();
                }, "UserService", new UserDto());

                property.Owner = owner;

                return property;
            }, TimeSpan.FromMinutes(10));

            if (propertyDetails == null)
                return NotFound($"Property {propertyId} not found.");

            return Ok(propertyDetails);
        }


        [HttpGet("room-details/{roomId:guid}")]
        public async Task<IActionResult> GetRoomDetails(Guid roomId)
        {
            var cacheKey = $"room-details-{roomId}";
            var propertyClient = _httpClientFactory.CreateClient("PropertyClient");
            var reviewClient = _httpClientFactory.CreateClient("ReviewClient");
            var userClient = _httpClientFactory.CreateClient("UserClient");

            var roomDetails = await GetOrSetCacheAsync(cacheKey, async () =>
            {
                var roomDetailsResponse = await propertyClient.GetAsync($"/api/Property/rooms/{roomId}/details");
                if (!roomDetailsResponse.IsSuccessStatusCode) return null;

                var room = await roomDetailsResponse.Content.ReadFromJsonAsync<RoomWithPropertyDetailsDto>();
                if (room == null) return null;

                if (room.OwnerId.HasValue)
                {
                    var owner = await SafeCall(async () =>
                    {
                        var ownerResp = await userClient.GetAsync($"/api/User/{room.OwnerId.Value}");
                        if (!ownerResp.IsSuccessStatusCode) return null;
                        return await ownerResp.Content.ReadFromJsonAsync<UserDto>();
                    }, "UserService", null);

                    if (owner != null)
                    {
                        room.Owner = new OwnerInfoDto
                        {
                            FirstName = owner.FirstName,
                            LastName = owner.LastName,
                            Email = owner.Email
                        };
                    }
                }

                var reviews = await SafeCall(async () =>
                {
                    var revResp = await reviewClient.GetAsync($"/api/Review/room/{roomId}");
                    if (!revResp.IsSuccessStatusCode) return new List<ReviewDto>();
                    var revs = await revResp.Content.ReadFromJsonAsync<List<ReviewDto>>();
                    return revs ?? new List<ReviewDto>();
                }, "ReviewService", new List<ReviewDto>());

                if (reviews.Any())
                {
                    var reviewerIds = reviews.Select(r => r.ReviewerId).Distinct();
                    var users = await Task.WhenAll(reviewerIds.Select(async id =>
                    {
                        return await SafeCall(async () =>
                        {
                            var userResp = await userClient.GetAsync($"/api/User/{id}");
                            if (!userResp.IsSuccessStatusCode) return null;
                            return await userResp.Content.ReadFromJsonAsync<UserDto>();
                        }, "UserService", null);
                    }));

                    foreach (var review in reviews)
                    {
                        review.Reviewer = users.FirstOrDefault(u => u?.Id == review.ReviewerId);
                    }
                }

                room.Reviews = reviews;

                return room;
            }, TimeSpan.FromMinutes(10));

            if (roomDetails == null)
                return NotFound($"Room {roomId} not found.");

            return Ok(roomDetails);
        }


        [HttpGet("group-listing-details/{listingId:guid}")]
        public async Task<IActionResult> GetGroupListingDetails(Guid listingId)
        {
            var cacheKey = $"group-listing-details-{listingId}";
            var groupClient = _httpClientFactory.CreateClient("GroupClient");
            var propertyClient = _httpClientFactory.CreateClient("PropertyClient");
            var userClient = _httpClientFactory.CreateClient("UserClient");

            var listing = await GetOrSetCacheAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching group listing details for {ListingId}", listingId);

                var listingResponse = await groupClient.GetAsync($"/api/Group/listings/{listingId}");
                if (!listingResponse.IsSuccessStatusCode) return null;

                var listingDto = await listingResponse.Content.ReadFromJsonAsync<GroupListingDto>();
                if (listingDto == null) return null;

                if (listingDto.PropertyId.HasValue && listingDto.Property == null)
                {
                    var property = await SafeCall(async () =>
                    {
                        var resp = await propertyClient.GetAsync($"/api/Property/{listingDto.PropertyId.Value}");
                        if (!resp.IsSuccessStatusCode) return null;
                        return await resp.Content.ReadFromJsonAsync<PropertyDto>();
                    }, "PropertyService", null);

                    listingDto.Property = property;
                }

                if (listingDto.RoomId.HasValue && listingDto.Room == null)
                {
                    var room = await SafeCall(async () =>
                    {
                        var resp = await propertyClient.GetAsync($"/api/Property/rooms/{listingDto.RoomId.Value}/details");
                        if (!resp.IsSuccessStatusCode) return null;
                        return await resp.Content.ReadFromJsonAsync<RoomWithPropertyDetailsDto>();
                    }, "PropertyService", null);

                    listingDto.Room = room;
                }

                if (listingDto.GroupId != Guid.Empty && listingDto.Group == null)
                {
                    var group = await SafeCall(async () =>
                    {
                        var resp = await groupClient.GetAsync($"/api/Group/{listingDto.GroupId}");
                        if (!resp.IsSuccessStatusCode) return null;
                        return await resp.Content.ReadFromJsonAsync<GroupDto>();
                    }, "GroupService", null);

                    listingDto.Group = group;

                    if (group?.Members?.Any() == true)
                    {
                        await EnrichGroupMembersWithUserDetails(group, userClient);
                    }
                }
                else if (listingDto.Group?.Members?.Any() == true)
                {
                    await EnrichGroupMembersWithUserDetails(listingDto.Group, userClient);
                }

                if (listingDto.Applications?.Any() == true)
                {
                    await EnrichApplicationsWithUserDetails(listingDto.Applications, userClient);
                }

                return listingDto;
            }, TimeSpan.FromMinutes(5));

            if (listing == null)
                return NotFound($"Group listing {listingId} not found.");

            return Ok(listing);
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