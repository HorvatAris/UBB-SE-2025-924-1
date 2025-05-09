// <copyright file="IGameServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Proxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Text;
    using System.Threading.Tasks;
    using Refit;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Repositories;
    using System.Text.Json.Serialization;

    public class GameRepositoryProxy : IGameRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } };

        public GameRepositoryProxy()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7241"); // Adjust to your actual backend URL
        }

        public async Task<GameDetailedResponse> CreateGameAsync(CreateGameRequest game)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Games", game);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GameDetailedResponse>(_options)
                   ?? throw new InvalidOperationException("Invalid response from CreateGame");
        }

        public async Task<GameDetailedResponse?> GetGameByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Games/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GameDetailedResponse>(_options);
        }

        public async Task<List<GameDetailedResponse>> GetGamesAsync(GetGamesRequest request)
        {
            // Build query string manually
            var query = new List<string>();
            if (request.StatusIs != null) query.Add($"StatusIs={request.StatusIs}");
            if (request.PublisherIdentifierIs != null) query.Add($"PublisherIdentifierIs={request.PublisherIdentifierIs}");
            if (request.PublisherIdentifierIsnt != null) query.Add($"PublisherIdentifierIsnt={request.PublisherIdentifierIsnt}");

            var url = "/api/Games";
            if (query.Any())
                url += "?" + string.Join("&", query);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GameDetailedResponse>>(_options)
                   ?? new List<GameDetailedResponse>();
        }

        public async Task DeleteGameAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Games/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateGameAsync(int id, UpdateGameRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/Games/{id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task PatchGameTagsAsync(int id, PatchGameTagsRequest tags)
        {
            var json = JsonSerializer.Serialize(tags);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"/api/Games/{id}/tags", content);
            response.EnsureSuccessStatusCode();
        }
    }
}