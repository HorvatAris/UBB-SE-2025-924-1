// <copyright file="IITemTradeServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Proxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json.Serialization;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Refit;
    using SteamHub.ApiContract.Models.ItemTrade;
    using SteamHub.ApiContract.Repositories;
    using System.Net.Http;
    using System.Net.Http.Json;

    public class ItemTradeRepositoryProxy : IItemTradeRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // For Enum handling
        };

        public ItemTradeRepositoryProxy()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7241") // Replace with your actual base URL
            };
        }

        public async Task<GetItemTradesResponse?> GetItemTradesAsync()
        {
            var response = await _httpClient.GetAsync("/api/ItemTrades");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<GetItemTradesResponse>(_options);
            return result ?? new GetItemTradesResponse();
        }

        public async Task<ItemTradeResponse?> GetItemTradeByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/ItemTrades/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
            var result = await response.Content.ReadFromJsonAsync<ItemTradeResponse>(_options);
            return result;
        }

        public async Task<CreateItemTradeResponse> CreateItemTradeAsync(CreateItemTradeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/ItemTrades", request);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<CreateItemTradeResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from CreateItemTradeAsync");
        }

        public async Task UpdateItemTradeAsync(int id, UpdateItemTradeRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/ItemTrades/{id}", request);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
        }

        public async Task DeleteItemTradeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/ItemTrades/{id}");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
        }
    }
}
