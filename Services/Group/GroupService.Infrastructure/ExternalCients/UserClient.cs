using GroupService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Dto;
using Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroupService.Infrastructure.ExternalCients
{
    public class UserClient : IUserClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserClient> _logger;

        public UserClient(HttpClient httpClient, ILogger<UserClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/User/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                _logger.LogWarning("User not found: {UserId}, status: {StatusCode}", userId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user {UserId}", userId);
                return null;
            }
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/user/by-email/{Uri.EscapeDataString(email)}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                _logger.LogWarning("User not found: {Email}, status: {StatusCode}", email, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {Email}", email);
                return null;
            }

        }
    }
}
