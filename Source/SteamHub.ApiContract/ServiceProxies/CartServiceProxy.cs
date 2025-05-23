using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Models.UsersGames;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class CartServiceProxy : ICartService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public CartServiceProxy(IHttpClientFactory httpClientFactory, IUserDetails user)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.user = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        private const int InitialZeroSum = 0;
        private IUserDetails user;

        public async Task AddGameToCartAsync(UserGameRequest gameRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Cart/AddToCart", gameRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = (await response.Content.ReadAsStringAsync()).Trim('"');
                    System.Diagnostics.Debug.WriteLine($"API returned error: {errorMessage}");
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding game from cart: {exception.Message}");
                throw;
            }
        }

        public async Task<List<int>> GetAllCartGamesIdsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Cart/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code

                var result = await response.Content.ReadFromJsonAsync<List<Game>>(_options);

                var gameIds = result
                    .Select(currentGame => currentGame.GameId)
                    .ToList();
                return gameIds ?? new List<int>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<int>();
            }
        }

        public async Task<List<Game>> GetAllPurchasedGamesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Cart/Purchased/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code
                var purchasedGames = await response.Content.ReadFromJsonAsync<List<Game>>(_options);

                return purchasedGames ?? new List<Game>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<Game>();
            }
        }

        public async Task<List<Game>> GetCartGamesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Cart/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code
                var games = await response.Content.ReadFromJsonAsync<List<Game>>(_options);
               
                return games ?? new List<Game>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
                return new List<Game>();
            }
        }

        public float GetTheTotalSumOfItemsInCart(List<Game> cartGames)
        {
            float totalSum = InitialZeroSum;
            foreach (var game in cartGames)
            {
                totalSum += (float)game.Price;
            }

            return totalSum;
        }

        public async Task<decimal> GetTotalSumToBePaidAsync()
        {
            decimal totalSumToBePaid = InitialZeroSum;
            var cartGames = await this.GetCartGamesAsync(this.user.UserId);

            foreach (var game in cartGames)
            {
                totalSumToBePaid += (decimal)game.Price;
            }

            return totalSumToBePaid;
        }

        public User GetUser()
        {
            return new User(this.user);
        }

        public float GetUserFunds()
        {
            return this.user.WalletBalance;
        }

        public async Task RemoveGameFromCartAsync(UserGameRequest gameRequest)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync("/api/Cart/RemoveFromCart", gameRequest);
                response.EnsureSuccessStatusCode(); // Ensure successful status code
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing game from cart: {exception.Message}");
            }
        }

        public async Task RemoveGamesFromCartAsync(List<Game> games)
        {
            var gameRequest = new UserGameRequest
            {
                UserId = this.user.UserId
            };

            foreach (var game in games)
            {
                gameRequest.GameId = game.GameId;
                await this.RemoveGameFromCartAsync(gameRequest);
            }
        }
    }
}
