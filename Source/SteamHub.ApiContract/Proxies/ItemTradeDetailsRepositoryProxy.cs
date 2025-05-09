// <copyright file="IItemTradeDetailServiceProxy.cs" company="PlaceholderCompany">
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
    using SteamHub.ApiContract.Models.ItemTradeDetails;
    using SteamHub.ApiContract.Repositories;

    public class ItemTradeDetailsRepositoryProxy : IItemTradeDetailRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public ItemTradeDetailsRepositoryProxy()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7241") // Adjust to your actual backend URL
            };
        }

        public async Task<GetItemTradeDetailsResponse?> GetItemTradeDetailsAsync()
        {
            var response = await _httpClient.GetAsync("/api/ItemTradeDetails");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<GetItemTradeDetailsResponse>(_options);
            return result ?? new GetItemTradeDetailsResponse();
        }

        public async Task<ItemTradeDetailResponse?> GetItemTradeDetailAsync(int tradeId, int itemId)
        {
            var response = await _httpClient.GetAsync($"/api/ItemTradeDetails/{tradeId}/{itemId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
            var result = await response.Content.ReadFromJsonAsync<ItemTradeDetailResponse>(_options);
            return result;
        }

        public async Task<CreateItemTradeDetailResponse> CreateItemTradeDetailAsync(CreateItemTradeDetailRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/ItemTradeDetails", request);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)

            var result = await response.Content.ReadFromJsonAsync<CreateItemTradeDetailResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from CreateItemTradeDetailAsync");
        }

        public async Task DeleteItemTradeDetailAsync(int tradeId, int itemId)
        {
            var response = await _httpClient.DeleteAsync($"/api/ItemTradeDetails/{tradeId}/{itemId}");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx)
        }
    }
}
