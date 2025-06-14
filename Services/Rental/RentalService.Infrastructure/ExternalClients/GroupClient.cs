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
    public class GroupClient : IGroupClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GroupClient> _logger;

        public GroupClient(HttpClient httpClient, ILogger<GroupClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GroupDto?> GetGroupByIdAsync(Guid groupId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Group/{groupId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<GroupDto>();
                }

                _logger.LogWarning("Group not found: {GroupId}, status: {StatusCode}", groupId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching group {GroupId}", groupId);
                return null;
            }
        }
    }
}
