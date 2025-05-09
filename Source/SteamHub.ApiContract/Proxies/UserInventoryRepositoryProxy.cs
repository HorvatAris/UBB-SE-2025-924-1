// <copyright file="IUserInventoryServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Proxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamHub.ApiContract.Repositories;

    public class UserInventoryRepositoryProxy : IUserInventoryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public UserInventoryRepositoryProxy()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7241") 
            };
        }

        public async Task<UserInventoryResponse> GetUserInventoryAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/UserInventory/{userId}");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<UserInventoryResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from GetUserInventoryAsync");
        }

        public async Task<InventoryItemResponse?> GetItemFromUserInventoryAsync(int userId, int itemId)
        {
            var response = await _httpClient.GetAsync($"/api/UserInventory/{userId}/item/{itemId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
            var result = await response.Content.ReadFromJsonAsync<InventoryItemResponse>(_options);
            return result;
        }

        public async Task AddItemToUserInventoryAsync(ItemFromInventoryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/UserInventory", request);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
        }

        public async Task RemoveItemFromUserInventoryAsync(ItemFromInventoryRequest request)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/api/UserInventory", UriKind.Relative),
                Content = JsonContent.Create(request, options: _options)
            };

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }
    }
}
