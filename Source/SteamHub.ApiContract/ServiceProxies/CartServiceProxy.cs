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

        public async Task AddGameToCartAsync(Game game)
        {
            var purchasedGames = await this.GetAllPurchasedGamesAsync();
            var cartGamesIds = await this.GetAllCartGamesIdsAsync();
            foreach (var purchasedGame in purchasedGames)
            {
                if (game.GameId == purchasedGame.GameId)
                {
                    // System.Diagnostics.Debug.WriteLine("The game is already purchased.");
                    throw new Exception("The game is already purchased.");
                }
            }

            foreach (var gameId in cartGamesIds)
            {
                if (game.GameId == gameId)
                {
                    // System.Diagnostics.Debug.WriteLine("The game is already in the cart.");
                    throw new Exception("The game is already in the cart.");
                }
            }

            var request = new UserGameRequest
            {
                UserId = this.user.UserId,
                GameId = game.GameId,
            };

            System.Diagnostics.Debug.WriteLine("user id for adding to cart" + this.user.UserId);

            var response = await _httpClient.PostAsJsonAsync("api/UsersGames/AddToCart", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"AddToCart failed: {response.StatusCode} - {errorContent}");
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<int>> GetAllCartGamesIdsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/UsersGames/Cart/{this.user.UserId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code

                var result = await response.Content.ReadFromJsonAsync<GetUserGamesResponse>(_options);

                var userGamesResponses = result.UserGames; // Access the actual list here
                var gameIds = userGamesResponses
                    .Where(currentGame => currentGame.IsInCart)
                    .Select(currentGame => currentGame.GameId)
                    .ToList();
                return gameIds;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<int>();
            }
        }

        public async Task<List<Game>> GetAllPurchasedGamesAsync()
        {
            var purchasedGames = new List<Game>();
            try
            {
                var response = await _httpClient.GetAsync($"/api/UsersGames/Purchased/{this.user.UserId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code

                var result = await response.Content.ReadFromJsonAsync<GetUserGamesResponse>(_options);

                var userGamesResponses = result.UserGames; // Access the actual list here
                foreach (var userGame in userGamesResponses)
                {
                    var responseGame = await _httpClient.GetAsync($"/api/Games/{userGame.GameId}");

                    responseGame.EnsureSuccessStatusCode();
                    var resultGame = await responseGame.Content.ReadFromJsonAsync<GameDetailedResponse>(_options);
                    
                    var game = GameMapper.MapToGame(resultGame);
                    purchasedGames.Add(game);
                }

                return purchasedGames;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<Game>();
            }
        }

        public async Task<List<Game>> GetCartGamesAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"UserId: {this.user.UserId}");
                
                var response = await _httpClient.GetAsync($"/api/UsersGames/Cart/{this.user.UserId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code
                var result = await response.Content.ReadFromJsonAsync<GetUserGamesResponse>(_options);

                var userGamesResponses = result.UserGames; // Access the actual list here                var userGamesResponses = response.UserGames; // Access the actual list her
                System.Diagnostics.Debug.WriteLine($"UserGamesResponses: {userGamesResponses.Count}");
                var gameIds = userGamesResponses
            .Select(game => game.GameId)
            .ToList();
                if (gameIds.Count == 0)
                {
                    return new List<Game>();
                }

                var games = new List<Game>();
                foreach (var gameId in gameIds)
                {
                    System.Diagnostics.Debug.WriteLine($"GameId: {gameId}");
                    var responseGame = await _httpClient.GetAsync($"/api/Games/{gameId}");

                    response.EnsureSuccessStatusCode();
                    var resultGame = await responseGame.Content.ReadFromJsonAsync<GameDetailedResponse>(_options);

                    var game = GameMapper.MapToGame(resultGame);
                    games.Add(game);
                }

                return games;
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
            var cartGames = await this.GetCartGamesAsync();

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

        public async Task RemoveGameFromCartAsync(Game game)
        {
            try
            {
                var request = new UserGameRequest
                {
                    UserId = this.user.UserId,
                    GameId = game.GameId,
                };

                var response = await _httpClient.PatchAsJsonAsync("/api/UsersGames/RemoveFromCart", request);
                response.EnsureSuccessStatusCode(); // Ensure successful status code
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing game from cart: {exception.Message}");
            }
        }

        public async Task RemoveGamesFromCartAsync(List<Game> games)
        {
            foreach (var game in games)
            {
                await this.RemoveGameFromCartAsync(game);
            }
        }
    }
}
