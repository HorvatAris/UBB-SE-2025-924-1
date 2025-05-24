using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.User;
using System.Net.Http.Json;

namespace SteamHub.ApiContract.ServiceProxies
{

    public class UserServiceProxy :  IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        public UserServiceProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync("/api/User/All");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch users: {errorContent}");
            }

            var content = await response.Content.ReadFromJsonAsync<List<User>>(_options);
            return content!;
        }
    }
}
