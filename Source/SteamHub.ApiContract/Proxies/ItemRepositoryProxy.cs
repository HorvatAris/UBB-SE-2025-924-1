// <copyright file="IItemServiceProxy.cs" company="PlaceholderCompany">
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
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Repositories;

    public class ItemRepositoryProxy : IItemRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Ensures proper enum parsing
        };

        public ItemRepositoryProxy()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7241") // Replace with your actual base URL
            };
        }

        public async Task<IEnumerable<ItemDetailedResponse>> GetItemsAsync()
        {
            var response = await _httpClient.GetAsync("/api/Items");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ItemDetailedResponse>>(_options);
            return result ?? new List<ItemDetailedResponse>();
        }

        public async Task<ItemDetailedResponse> GetItemByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Items/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
            var result = await response.Content.ReadFromJsonAsync<ItemDetailedResponse>(_options);
            return result;
        }

        public async Task<ItemDetailedResponse> CreateItemAsync(CreateItemRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Items", request);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<ItemDetailedResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from CreateItemAsync");
        }

        public async Task UpdateItemAsync(int id, UpdateItemRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Items/{id}", request);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
        }

        public async Task DeleteItemAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Items/{id}");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
        }
    }
}
