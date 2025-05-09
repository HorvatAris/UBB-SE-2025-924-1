using SteamHub.ApiContract.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Repositories
{
    public class GameRepositoryProxy : IGameRepository
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public GameRepositoryProxy()
        {
            _http = new HttpClient();
            _http.BaseAddress = new Uri("https://localhost:7241");
        }

        public async Task<GameDetailedResponse> CreateGameAsync(CreateGameRequest game)
        {
            var response = await _http.PostAsJsonAsync("/api/Games", game);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GameDetailedResponse>(_jsonOptions);
        }

        public async Task<GameDetailedResponse?> GetGameByIdAsync(int id)
        {
            var response = await _http.GetAsync($"/api/Games/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GameDetailedResponse>(_jsonOptions);
        }

        public async Task<List<GameDetailedResponse>> GetGamesAsync(GetGamesRequest request)
        {
            var queryParams = new List<string>();
            if (request.StatusIs.HasValue)
                queryParams.Add($"StatusIs={(int)request.StatusIs.Value}");
            if (request.PublisherIdentifierIs.HasValue)
                queryParams.Add($"PublisherIdentifierIs={request.PublisherIdentifierIs.Value}");
            if (request.PublisherIdentifierIsnt.HasValue)
                queryParams.Add($"PublisherIdentifierIsnt={request.PublisherIdentifierIsnt.Value}");

            var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

            var response = await _http.GetAsync("/api/Games" + query);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<GameDetailedResponse>>(_jsonOptions);
        }

        public async Task UpdateGameAsync(int id, UpdateGameRequest game)
        {
            var response = await _http.PatchAsync($"/api/Games/{id}", JsonContent.Create(game));
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteGameAsync(int id)
        {
            var response = await _http.DeleteAsync($"/api/Games/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task PatchGameTagsAsync(int id, PatchGameTagsRequest tags)
        {
            var response = await _http.PatchAsync($"/api/Games/{id}/tags", JsonContent.Create(tags));
            response.EnsureSuccessStatusCode();
        }
    }
}
