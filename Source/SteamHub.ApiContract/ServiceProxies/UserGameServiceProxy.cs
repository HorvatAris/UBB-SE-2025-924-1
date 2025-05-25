using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Constants;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Models.UsersGames;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class UserGameServiceProxy : IUserGameService
    {
        private const int InitialValueForLastEarnedPoints = 0;
        private const int ResetValueForNumberOfUserGamesWithTag = 0;
        private const int NumberOfFavouriteTagsToTake = 3;
        private const int StartingIndexValue = 0;
        private const int InitialTagScore = 0;
        private const int TagScoreMultiplierNumerator = 1;
        private const decimal TagScoreMultiplierDenominator = 3m;
        private const decimal WeightedScoreMultiplier = 0.5m;
        private const int NumberOfSortegGamesShown = 10;
        private const decimal MinimumValueForOverwhelminglyPositive = 4.5m;
        private const decimal MinimumValueForVeryPositive = 4m;
        private const decimal MinimumValueForMixed = 2m;
        private const int ValueToDecrementPositionWith = 1;
        private const int ValueToIncrementPositionWith = 1;

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        public UserGameServiceProxy(IHttpClientFactory httpClientFactory, IUserDetails user)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.User = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        public IUserDetails User { get; set; }
        public int LastEarnedPoints { get; private set; }

        public async Task AddGameToWishlistAsync(UserGameRequest gameRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/UserGame/AddToWishlist", gameRequest);
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = (await response.Content.ReadAsStringAsync()).Trim('"');
                    System.Diagnostics.Debug.WriteLine($"API returned error: {errorMessage}");
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding game to wishlist: {exception.Message}");
                throw;
            }
        }

        public async Task<Collection<Game>> GetAllGamesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/UserGame/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code
                var result = await response.Content.ReadFromJsonAsync<Collection<Game>>(_options);
                return result ?? throw new InvalidOperationException("Invalid response from GetAllGamesAsync");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
                return new Collection<Game>();
            }
        }

        public async Task ComputeNoOfUserGamesForEachTagAsync(Collection<Tag> all_tags, int userId)
        {
            var user_games = await this.GetAllGamesAsync(User.UserId);

            // Manually build the dictionary instead of using ToDictionary
            Dictionary<string, Tag> tagsDictionary = new Dictionary<string, Tag>();
            foreach (var tag in all_tags)
            {
                if (!tagsDictionary.ContainsKey(tag.Tag_name))
                {
                    tagsDictionary.Add(tag.Tag_name, tag);
                }
            }

            foreach (var tag in tagsDictionary.Values)
            {
                tag.NumberOfUserGamesWithTag = ResetValueForNumberOfUserGamesWithTag;
            }

            foreach (var user_game in user_games)
            {
                foreach (string tag_name in user_game.Tags)
                {
                    if (tagsDictionary.ContainsKey(tag_name))
                    {
                        tagsDictionary[tag_name].NumberOfUserGamesWithTag++;
                    }
                }
            }
        }

        public async Task ComputeTagScoreForGamesAsync(Collection<Game> games, int userId)
        {
            var favorite_tags = await this.GetFavoriteUserTagsAsync(User.UserId);
            foreach (var game in games)
            {
                game.TagScore = InitialTagScore;
                foreach (var tag in favorite_tags)
                {
                    if (game.Tags.Contains(tag.Tag_name))
                    {
                        game.TagScore += tag.NumberOfUserGamesWithTag;
                    }
                }

                game.TagScore = game.TagScore * (TagScoreMultiplierNumerator / TagScoreMultiplierDenominator);
            }
        }

        public void ComputeTrendingScores(Collection<Game> games)
        {
            var maxRecentSales = games.Max(game => game.NumberOfRecentPurchases);
            foreach (var game in games)
            {
                game.TrendingScore = Convert.ToDecimal(game.NumberOfRecentPurchases) / maxRecentSales;
            }
        }

        public async Task<Collection<Game>> FilterWishListGamesAsync(string criteria)
        {
            Collection<Game> games = await this.GetWishListGamesAsync(User.UserId);
            Collection<Game> filteredGames = new Collection<Game>();

            bool isKnownCriteria = criteria == FilterCriteria.OVERWHELMINGLYPOSITIVE ||
                                   criteria == FilterCriteria.VERYPOSITIVE ||
                                   criteria == FilterCriteria.MIXED ||
                                   criteria == FilterCriteria.NEGATIVE;

            if (!isKnownCriteria)
            {
                // If the criteria is not recognized, return the full list
                return games;
            }

            foreach (Game game in games)
            {
                if (criteria == FilterCriteria.OVERWHELMINGLYPOSITIVE && game.Rating >= MinimumValueForOverwhelminglyPositive)
                {
                    filteredGames.Add(game);
                }
                else if (criteria == FilterCriteria.VERYPOSITIVE &&
                         game.Rating >= MinimumValueForVeryPositive &&
                         game.Rating < MinimumValueForOverwhelminglyPositive)
                {
                    filteredGames.Add(game);
                }
                else if (criteria == FilterCriteria.MIXED &&
                         game.Rating >= MinimumValueForMixed &&
                         game.Rating < MinimumValueForVeryPositive)
                {
                    filteredGames.Add(game);
                }
                else if (criteria == FilterCriteria.NEGATIVE &&
                         game.Rating < MinimumValueForMixed)
                {
                    filteredGames.Add(game);
                }
            }

            return filteredGames;
        }

        public async Task<Collection<Tag>> GetFavoriteUserTagsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/UserGame/Tags/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code
                var result = await response.Content.ReadFromJsonAsync<Collection<Tag>>(_options);
                return result ?? throw new InvalidOperationException("Invalid response from GetFavoriteUserTagsAsync");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching favorite user tags: {exception.Message}");
                return new Collection<Tag>();
            }
        }

        public async Task<Collection<Game>> GetRecommendedGamesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/UserGame/RecommendedGames/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code
                var result = await response.Content.ReadFromJsonAsync<Collection<Game>>(_options);
                return result ?? throw new InvalidOperationException("Invalid response from GetRecommendedGamesAsync");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching recommended games: {exception.Message}");
                return new Collection<Game>();
            }
        }

        public async Task<Collection<Game>> GetWishListGamesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/UserGame/Wishlist/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code
                var result = await response.Content.ReadFromJsonAsync<Collection<Game>>(_options);
                return result ?? throw new InvalidOperationException("Invalid response from GetWishListGamesAsync");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
                return new Collection<Game>();
            }
        }

        public async Task<bool> IsGamePurchasedAsync(Game game, int userId)
        {
            var purchasedGameList = await this.GetPurchasedGamesAsync(User.UserId);
            return purchasedGameList.Any(currentGame => currentGame.GameId == game.GameId);
        }

        public async Task<Collection<Game>> GetPurchasedGamesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/UserGame/Purchased/{userId}");
                response.EnsureSuccessStatusCode(); // Ensure successful status code
                var result = await response.Content.ReadFromJsonAsync<Collection<Game>>(_options);
                return result ?? throw new InvalidOperationException("Invalid response from GetPurchasedGamesAsync");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
                return new Collection<Game>();
            }
        }

        public async Task<int> PurchaseGamesAsync(PurchaseGamesRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/UserGame/Purchase", request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PurchaseResponse>(responseContent, _options);
                if (result != null)
                {
                    LastEarnedPoints = result.PointsEarned;
                    User.PointsBalance += result.PointsEarned;
                }

                return result?.PointsEarned ?? InitialValueForLastEarnedPoints;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error purchasing games: {exception.Message}");
                throw;
            }
        }

        public Task RemoveGameFromWishlistAsync(UserGameRequest gameRequest)
        {
            try
            {
                var response = _httpClient.PatchAsJsonAsync("/api/UserGame/RemoveFromWishlist", gameRequest);
                response.Result.EnsureSuccessStatusCode(); // Ensure successful status code
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing game from wishlist: {exception.Message}");
                throw;
            }
        }

        public async Task<Collection<Game>> SearchWishListByNameAsync(string searchText)
        {
            List<Game> allWishListGames = (await this.GetWishListGamesAsync(User.UserId)).ToList();
            List<Game> matchingGames = new List<Game>();

            foreach (Game game in allWishListGames)
            {
                if (game.GameTitle != null && game.GameTitle.ToLower().Contains(searchText.ToLower()))
                {
                    matchingGames.Add(game);
                }
            }

            return new Collection<Game>(matchingGames);
        }

        public async Task<Collection<Game>> SortWishListGamesAsync(string criteria, bool ascending)
        {
            Collection<Game> gamesCollection = await this.GetWishListGamesAsync(User.UserId);
            List<Game> games = new List<Game>();

            foreach (var game in gamesCollection)
            {
                games.Add(game);
            }

            if (criteria == FilterCriteria.PRICE)
            {
                if (ascending)
                {
                    games.Sort(this.CompareByPriceAscending);
                }
                else
                {
                    games.Sort(this.CompareByPriceDescending);
                }
            }
            else if (criteria == FilterCriteria.RATING)
            {
                if (ascending)
                {
                    games.Sort(this.CompareByRatingAscending);
                }
                else
                {
                    games.Sort(this.CompareByRatingDescending);
                }
            }
            else if (criteria == FilterCriteria.DISCOUNT)
            {
                if (ascending)
                {
                    games.Sort(this.CompareByDiscountAscending);
                }
                else
                {
                    games.Sort(this.CompareByDiscountDescending);
                }
            }
            else
            {
                if (ascending)
                {
                    games.Sort(this.CompareByNameAscending);
                }
                else
                {
                    games.Sort(this.CompareByNameDescending);
                }
            }

            return new Collection<Game>(games);
        }

        private int CompareByPriceAscending(Game firstGame, Game secondGame)
        {
            return firstGame.Price.CompareTo(secondGame.Price);
        }

        private int CompareByPriceDescending(Game firstGame, Game secondGame)
        {
            return secondGame.Price.CompareTo(firstGame.Price);
        }

        private int CompareByRatingAscending(Game firstGame, Game secondGame)
        {
            return firstGame.Rating.CompareTo(secondGame.Rating);
        }

        private int CompareByRatingDescending(Game firstGame, Game secondGame)
        {
            return secondGame.Rating.CompareTo(firstGame.Rating);
        }

        private int CompareByDiscountAscending(Game firstGame, Game secondGame)
        {
            return firstGame.Discount.CompareTo(secondGame.Discount);
        }

        private int CompareByDiscountDescending(Game firstGame, Game secondGame)
        {
            return secondGame.Discount.CompareTo(firstGame.Discount);
        }

        private int CompareByNameAscending(Game firstGame, Game secondGame)
        {
            return string.Compare(firstGame.GameTitle, secondGame.GameTitle, StringComparison.OrdinalIgnoreCase);
        }

        private int CompareByNameDescending(Game firstGame, Game secondGame)
        {
            return string.Compare(secondGame.GameTitle, firstGame.GameTitle, StringComparison.OrdinalIgnoreCase);
        }

        public IUserDetails GetUser()
        {
            return this.User;
        }
    }
}
