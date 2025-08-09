using Microsoft.Extensions.Logging;
using RentalService.Application.Interfaces;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Infrastructure.ExternalClients
{
    public class PropertyClient : IPropertyClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserClient> _logger;

        public PropertyClient(HttpClient httpClient, ILogger<UserClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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

        public async Task<RoomDto?> GetRoomByIdAsync(Guid roomId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Property/rooms/{roomId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RoomDto>();
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
    }
}
