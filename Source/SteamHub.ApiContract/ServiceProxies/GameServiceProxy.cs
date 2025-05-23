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
using SteamHub.ApiContract.Models.PointShopItem;
using System.Net.Http.Json;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class GameServiceProxy : IGameService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        private const int MinimumTrendingDivider = 1;
        private const decimal NoTrendingScore = 0m;
        private const int NumberOfSimilarGamesToTake = 3;
        private static int lengthOfEmptyList = 0;
        private static int initializingValueForAMaxim = 0;
        private static int treasholdForDiscount = 0;
        private static int startingValueOfIndex = 0;
        private static int incrementingValue = 1;
        private static int numberOfGamesToTake = 10;

        public GameServiceProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }
        public void ComputeTrendingScores(Collection<Game> games)
        {
            int maximumRecentSales = initializingValueForAMaxim;

            foreach (var game in games)
            {
                if (game.NumberOfRecentPurchases > maximumRecentSales)
                {
                    maximumRecentSales = game.NumberOfRecentPurchases;
                }
            }

            foreach (var game in games)
            {
                game.TrendingScore = maximumRecentSales < MinimumTrendingDivider
                    ? NoTrendingScore
                    : Convert.ToDecimal(game.NumberOfRecentPurchases) / maximumRecentSales;
            }
        }

        public async Task<Collection<Game>> FilterGamesAsync(
            int minimumRating,
            int minimumPrice,
            int maximumPrice,
            string[] tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            var filteredGames = new List<Game>();

            var allGames = await this.GetAllApprovedGamesAsync();
            foreach (var game in allGames)
            {
                if (game.Rating >= minimumRating && game.Price >= minimumPrice && game.Price <= maximumPrice)
                {
                    bool hasAllTags = true;

                    if (tags.Length == 1 && tags[0] == null)
                    {
                        hasAllTags = true;
                    }
                    else if (tags.Length > lengthOfEmptyList)
                    {
                        foreach (var tag in tags)
                        {
                            if (!game.Tags.Contains(tag))
                            {
                                hasAllTags = false;
                                break; // No need to check further if one tag is missing
                            }
                        }
                    }

                    // If game has all tags or no tags were provided, add to the filtered list
                    if (hasAllTags)
                    {
                        filteredGames.Add(game);
                    }
                }
            }

            return new Collection<Game>(filteredGames);
        }

        public async Task<Collection<Game>> GetAllApprovedGamesAsync()
        {
            var allGames = await this.GetAllGamesAsync();
            var approvedGames = new Collection<Game>();
            foreach (var game in allGames)
            {
                if (game.Status == "Approved")
                {
                    approvedGames.Add(game);
                }
            }

            return approvedGames;
        }

        public async Task<Collection<Game>> GetAllGamesAsync()
        {
            try
            {
                var gamesResponse = await _httpClient.GetAsync("/api/Game");
                gamesResponse.EnsureSuccessStatusCode();
                var gamesResult = await gamesResponse.Content.ReadFromJsonAsync<Collection<Game>>(_options);

                // Ensure a non-null list is returned
                return gamesResult ?? new Collection<Game>();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving items: {exception.Message}", exception);
            }
        }

        public async Task<Collection<Tag>> GetAllGameTagsAsync(Game game)
        {
            var allTags = await this.GetAllTagsAsync();

            // Extract the result from the task
            var tagsForCurrentGame = new List<Tag>();

            foreach (var tag in allTags)
            {
                if (game.Tags.Contains(tag.Tag_name))
                {
                    tagsForCurrentGame.Add(tag);
                }
            }

            return new Collection<Tag>(tagsForCurrentGame);
        }

        public async Task<Collection<Tag>> GetAllTagsAsync()
        {
            try
            {
                var tagsResponse = await _httpClient.GetAsync("/api/Game/Tags");
                tagsResponse.EnsureSuccessStatusCode();
                var tagsResult = await tagsResponse.Content.ReadFromJsonAsync<Collection<Tag>>(_options);
                // Ensure a non-null list is returned
                return tagsResult ?? new Collection<Tag>();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving items: {exception.Message}", exception);
            }
        }

        public async Task<Collection<Game>> GetDiscountedGamesAsync()
        {
            var allGames = await this.GetAllApprovedGamesAsync();
            var discountedGames = new List<Game>();

            foreach (var game in allGames)
            {
                if (game.Discount > treasholdForDiscount)
                {
                    discountedGames.Add(game);
                }
            }

            return this.GetSortedAndFilteredVideoGames(new Collection<Game>(discountedGames));
        }

        public async Task<Game> GetGameByIdAsync(int gameId)
        {
            try
            {
                var gameResponse = await _httpClient.GetAsync($"/api/Game/{gameId}");
                gameResponse.EnsureSuccessStatusCode();
                var gameResult = await gameResponse.Content.ReadFromJsonAsync<Game>(_options);
                // Ensure a non-null list is returned
                return gameResult ?? new Game();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving items: {exception.Message}", exception);
            }
        }

        public async Task<List<Game>> GetSimilarGamesAsync(int gameId)
        {
            var randomGenerator = new Random(DateTime.Now.Millisecond);
            var allGames = await this.GetAllApprovedGamesAsync();
            var similarGames = new List<Game>();

            // Filter games with different identifiers
            foreach (var game in allGames)
            {
                if (game.GameId != gameId)
                {
                    similarGames.Add(game);
                }
            }

            // Shuffle the list
            for (int currentIndex = startingValueOfIndex; currentIndex < similarGames.Count; currentIndex++)
            {
                var randomIndex = randomGenerator.Next(
                    currentIndex,
                    similarGames.Count); // Get a random index from currentIndex to the end of the list
                var temporaryGame = similarGames[currentIndex];
                similarGames[currentIndex] = similarGames[randomIndex];
                similarGames[randomIndex] = temporaryGame;
            }

            // Return the first 3 games
            return similarGames.Take(NumberOfSimilarGamesToTake).ToList();
        }

        public async Task<Collection<Game>> GetTrendingGamesAsync()
        {
            var allGames = await this.GetAllApprovedGamesAsync();
            return this.GetSortedAndFilteredVideoGames(allGames);
        }

        public async Task<Collection<Game>> SearchGamesAsync(string searchQuery)
        {
            var allGames = await this.GetAllApprovedGamesAsync();
            var foundGames = new List<Game>();

            if (searchQuery == null)
            {
                return new Collection<Game>(allGames);
            }

            foreach (var game in allGames)
            {
                if (game.GameTitle.ToLower().Contains(searchQuery.ToLower()))
                {
                    foundGames.Add(game);
                }
            }

            return new Collection<Game>(foundGames);
        }

        private Collection<Game> GetSortedAndFilteredVideoGames(Collection<Game> games)
        {
            // Compute the trending scores for all the games
            this.ComputeTrendingScores(games);

            // Create a list to hold the sorted games
            List<Game> sortedGames = new List<Game>();

            // Manually sort the games by descending trending score
            for (int currentIndex = startingValueOfIndex; currentIndex < games.Count; currentIndex++)
            {
                for (int comparisonIndex = currentIndex + incrementingValue;
                     comparisonIndex < games.Count;
                     comparisonIndex++)
                {
                    if (games[currentIndex].TrendingScore < games[comparisonIndex].TrendingScore)
                    {
                        // Swap the games
                        var temporaryGame = games[currentIndex];
                        games[currentIndex] = games[comparisonIndex];
                        games[comparisonIndex] = temporaryGame;
                    }
                }
            }

            // Take the top 10 games after sorting
            for (int topGamesIndex = startingValueOfIndex;
                 topGamesIndex < numberOfGamesToTake && topGamesIndex < games.Count;
                 topGamesIndex++)
            {
                sortedGames.Add(games[topGamesIndex]);
            }

            return new Collection<Game>(sortedGames);
        }

    }
}
