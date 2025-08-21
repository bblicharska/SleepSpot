using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentalService.Application.Interfaces;
using Shared.Dto; 

public class PropertyClient : IPropertyClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PropertyClient> _logger;

    public PropertyClient(HttpClient httpClient, ILogger<PropertyClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(Guid propertyId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/Property/{propertyId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PropertyDto>();
            }

            _logger.LogWarning("Property not found: {PropertyId}, status: {StatusCode}", propertyId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching property {PropertyId}", propertyId);
            return null;
        }
    }

    public async Task<RoomWithPropertyDetailsDto?> GetRoomByIdAsync(Guid roomId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/Property/rooms/{roomId}/details");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RoomWithPropertyDetailsDto>();
            }

            _logger.LogWarning("Room not found: {RoomId}, status: {StatusCode}", roomId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching room {RoomId}", roomId);
            return null;
        }
    }

    public async Task UpdateRoomAvailabilityAsync(Guid roomId, bool isAvailable, DateTime? availableSince = null)
    {
        var dto = new AvailabilityUpdateDto
        {
            IsAvailable = isAvailable,
            AvailableSince = availableSince
        };

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Property/rooms/{roomId}/availability", dto);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Updated availability for room {RoomId}: IsAvailable={IsAvailable}, AvailableSince={AvailableSince}", roomId, isAvailable, availableSince);
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to update room availability for {RoomId}. Status: {StatusCode}, Response: {Response}", roomId, response.StatusCode, content);
            throw new HttpRequestException($"Failed to update room availability. Status: {response.StatusCode}, Response: {content}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability for room {RoomId}", roomId);
            throw;
        }
    }

    public async Task UpdatePropertyAvailabilityAsync(Guid propertyId, bool isAvailable, DateTime? availableSince = null)
    {
        var dto = new AvailabilityUpdateDto
        {
            IsAvailable = isAvailable,
            AvailableSince = availableSince
        };

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Property/{propertyId}/availability", dto);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Updated availability for property {PropertyId}: IsAvailable={IsAvailable}, AvailableSince={AvailableSince}", propertyId, isAvailable, availableSince);
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to update property availability for {PropertyId}. Status: {StatusCode}, Response: {Response}", propertyId, response.StatusCode, content);
            throw new HttpRequestException($"Failed to update property availability. Status: {response.StatusCode}, Response: {content}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability for property {PropertyId}", propertyId);
            throw;
        }
    }

    private class AvailabilityUpdateDto
    {
        public bool IsAvailable { get; set; }
        public DateTime? AvailableSince { get; set; }
    }
}
