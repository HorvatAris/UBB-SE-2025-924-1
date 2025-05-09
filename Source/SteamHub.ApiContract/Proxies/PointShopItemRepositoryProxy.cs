// <copyright file="IPointShopItemServiceProxy.cs" company="PlaceholderCompany">
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
    using SteamHub.ApiContract.Models.PointShopItem;
    using SteamHub.ApiContract.Repositories;

    public class PointShopItemRepositoryProxy : IPointShopItemRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public PointShopItemRepositoryProxy()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7241") // Replace with your actual base URL
            };
        }

        public async Task<CreatePointShopItemResponse> CreatePointShopItemAsync(CreatePointShopItemRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/PointShopItems", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CreatePointShopItemResponse>(_options)
                   ?? throw new InvalidOperationException("Invalid response from CreatePointShopItem");
        }

        public async Task DeletePointShopItemAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/PointShopItems/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<PointShopItemResponse?> GetPointShopItemByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/PointShopItems/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PointShopItemResponse>(_options);
        }

        public async Task<GetPointShopItemsResponse> GetPointShopItemsAsync()
        {
            var response = await _httpClient.GetAsync("/api/PointShopItems");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<GetPointShopItemsResponse>(_options)
                   ?? new GetPointShopItemsResponse();
        }

        public async Task UpdatePointShopItemAsync(int itemId, UpdatePointShopItemRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/PointShopItems/{itemId}", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
