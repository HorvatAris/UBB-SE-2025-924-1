// <copyright file="IUserGameServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Proxies
{
    using System.Text.Json.Serialization;
    using System.Text.Json;
    using System;
    using System.Threading.Tasks;
    using Refit;
    using SteamHub.ApiContract.Models.UsersGames;
    using SteamHub.ApiContract.Repositories;
    using System.Net.Http;
    using System.Net.Http.Json;

    public class UserGamesRepositoryProxy : IUsersGamesRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public UserGamesRepositoryProxy()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7241") // Adjust to your actual backend URL
            };
        }

        public async Task<GetUserGamesResponse> GetUserGamesAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/UsersGames/{userId}");
            response.EnsureSuccessStatusCode(); // Ensure successful status code

            var result = await response.Content.ReadFromJsonAsync<GetUserGamesResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from GetUserGamesAsync");
        }

        public async Task<GetUserGamesResponse> GetUserWishlistAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/UsersGames/Wishlist/{userId}");
            response.EnsureSuccessStatusCode(); // Ensure successful status code

            var result = await response.Content.ReadFromJsonAsync<GetUserGamesResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from GetUserWishlistAsync");
        }

        public async Task AddToWishlistAsync(UserGameRequest usersGames)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/UsersGames/AddToWishlist", usersGames);
            response.EnsureSuccessStatusCode(); // Ensure successful status code
        }

        public async Task RemoveFromWishlistAsync(UserGameRequest usersGames)
        {
            var response = await _httpClient.PatchAsJsonAsync("/api/UsersGames/RemoveFromWishlist", usersGames);
            response.EnsureSuccessStatusCode(); // Ensure successful status code
        }

        public async Task PurchaseGameAsync(UserGameRequest usersGames)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/UsersGames/Purchased", usersGames);
            response.EnsureSuccessStatusCode(); // Ensure successful status code
        }

        public async Task<GetUserGamesResponse> GetUserCartAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/UsersGames/Cart/{userId}");
            response.EnsureSuccessStatusCode(); // Ensure successful status code

            var result = await response.Content.ReadFromJsonAsync<GetUserGamesResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from GetUserCartAsync");
        }

        public async Task AddToCartAsync(UserGameRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/UsersGames/AddToCart", request);
            response.EnsureSuccessStatusCode(); // Ensure successful status code
        }

        public async Task RemoveFromCartAsync(UserGameRequest request)
        {
            var response = await _httpClient.PatchAsJsonAsync("/api/UsersGames/RemoveFromCart", request);
            response.EnsureSuccessStatusCode(); // Ensure successful status code
        }

        public async Task<GetUserGamesResponse> GetUserPurchasedGamesAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/UsersGames/Purchased/{userId}");
            response.EnsureSuccessStatusCode(); // Ensure successful status code

            var result = await response.Content.ReadFromJsonAsync<GetUserGamesResponse>(_options);
            return result ?? throw new InvalidOperationException("Invalid response from GetUserPurchasedGamesAsync");
        }
    }
}
