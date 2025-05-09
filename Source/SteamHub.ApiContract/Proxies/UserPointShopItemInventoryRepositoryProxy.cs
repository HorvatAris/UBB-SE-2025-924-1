// <copyright file="IUserPointShopItemInventoryServiceProxy.cs" company="PlaceholderCompany">
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
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Context.Repositories;
    using SteamHub.ApiContract.Models.UserPointShopItemInventory;

    public class UserPointShopItemInventoryRepositoryProxy : IUserPointShopItemInventoryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public UserPointShopItemInventoryRepositoryProxy()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7241"); // Adjust to your actual backend URL
        }

        public async Task<GetUserPointShopItemInventoryResponse> GetUserInventoryAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/UserPointShopItemInventory/{userId}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GetUserPointShopItemInventoryResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from GetUserInventoryAsync");
        }

        public async Task PurchaseItemAsync(PurchasePointShopItemRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/UserPointShopItemInventory/purchase", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateItemStatusAsync(UpdateUserPointShopItemInventoryRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync("/api/UserPointShopItemInventory/update", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task ResetUserInventoryAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"/api/UserPointShopItemInventory/reset/{userId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
