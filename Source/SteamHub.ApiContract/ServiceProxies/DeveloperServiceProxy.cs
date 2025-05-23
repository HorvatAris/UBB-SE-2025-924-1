using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Constants;
using System.Net.Http.Json;
using SteamHub.ApiContract.Models.PointShopItem;
using SteamHub.ApiContract.Models.Developer;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class DeveloperServiceProxy : IDeveloperService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        private const int ComparingValueForPositivePrice = 0;
        private const int ComparingValueForMinimumDicount = 0;
        private const int ComparingValueForMaximumDicount = 100;
        private const int EmptyListLength = 0;
        private const string PendingState = "Pending";

        public IUserDetails User { get; set; }
        public DeveloperServiceProxy(IHttpClientFactory httpClientFactory, IUserDetails user)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.User = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }
     
        public async Task CreateGameWithTagsAsync(Game game, IList<Tag> selectedTags,int userId)
        {
            await this.CreateGameAsync(game,userId);
        }

        public async Task<Game> CreateValidatedGameAsync(
        string gameIdText,
        string name,
        string priceText,
        string description,
        string imageUrl,
        string trailerUrl,
        string gameplayUrl,
        string minimumRequirement,
        string reccommendedRequirement,
        string discountText,
        IList<Tag> selectedTags,int userId)
        {
            var game = this.ValidateInputForAddingAGame(
                gameIdText,
                name,
                priceText,
                description,
                imageUrl,
                trailerUrl,
                gameplayUrl,
                minimumRequirement,
                reccommendedRequirement,
                discountText,
                selectedTags);

            if (await this.IsGameIdInUseAsync(game.GameId))
            {
                throw new Exception(ExceptionMessages.IdAlreadyInUse);
            }

            await this.CreateGameWithTagsAsync(game, selectedTags,userId);
            return game;
        }

        public async Task DeleteGameAsync(int gameId, ObservableCollection<Game> developerGames)
        {
            Game gameToRemove = null;
            foreach (var game in developerGames)
            {
                if (game.GameId == gameId)
                {
                    gameToRemove = game;
                    break;
                }
            }

            if (gameToRemove != null)
            {
                developerGames.Remove(gameToRemove);
            }

            // Perform the actual deletion logic
            await this.DeleteGameAsync(gameId);
        }

        public async Task DeleteGameTagsAsync(int gameId)
        {
           var response = await _httpClient.PatchAsync($"/api/Developer/games/{gameId}/tags", null);
            response.EnsureSuccessStatusCode();
        }

        public Game FindGameInObservableCollectionById(int gameId, ObservableCollection<Game> gameList)
        {
            foreach (Game game in gameList)
            {
                if (game.GameId == gameId)
                {
                    return game;
                }
            }

            return null;
        }

        public async Task<Collection<Tag>> GetAllTagsAsync()
        {
            var response = await _httpClient.GetAsync("/api/Developer/tags");
            response.EnsureSuccessStatusCode();

            var tags = await response.Content.ReadFromJsonAsync<Collection<Tag>>();
            return tags ?? new Collection<Tag>();
        }


        public async Task<List<Game>> GetDeveloperGamesAsync()
        {
            var response = await _httpClient.GetAsync("/api/Developer/games/developer");
            response.EnsureSuccessStatusCode();

            var games = await response.Content.ReadFromJsonAsync<List<Game>>();
            return games ?? new List<Game>();
        }


        public async Task<int> GetGameOwnerCountAsync(int gameId)
        {
            var response = await _httpClient.GetAsync($"/api/Developer/games/{gameId}/owners/count");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }


        public async Task<List<Tag>> GetGameTagsAsync(int gameId)
        {
            var response = await _httpClient.GetAsync($"/api/Developer/games/{gameId}/tags");

            response.EnsureSuccessStatusCode();

            var tags = await response.Content.ReadFromJsonAsync<List<Tag>>();
            return tags ?? new List<Tag>();
        }


        public async Task<IList<Tag>> GetMatchingTagsForGameAsync(int gameId, IList<Tag> allAvailableTags)
        {
            List<Tag> matchedTags = new List<Tag>();
            List<Tag> gameTags = await this.GetGameTagsAsync(gameId);

            foreach (Tag tag in allAvailableTags)
            {
                foreach (Tag gameTag in gameTags)
                {
                    if (tag.TagId == gameTag.TagId)
                    {
                        matchedTags.Add(tag);
                        break;
                    }
                }
            }

            return matchedTags;
        }
        public async Task<string> GetRejectionMessageAsync(int game_id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Developer/games/{game_id}/rejection-message");
                response.EnsureSuccessStatusCode();

                var message = await response.Content.ReadAsStringAsync();
                return message;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching rejection message: {ex.Message}", ex);
            }
        }


        public async Task<List<Game>> GetUnvalidatedAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Developer/games/unvalidated");
                response.EnsureSuccessStatusCode();
                var userGames = await response.Content.ReadFromJsonAsync <List<Game>>(_options);

                return userGames ?? new List<Game>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching unvalidated games: {ex.Message}", ex);
            }

        }

        public async Task InsertGameTagAsync(int gameId, int tagId)
        {
            var response = await _httpClient.PostAsync($"/api/Developer/games/{gameId}/tags/{tagId}", null);
            response.EnsureSuccessStatusCode();
        }


        public async Task<bool> IsGameIdInUseAsync(int gameId)
        {
            var response = await _httpClient.GetAsync($"/api/Developer/games/{gameId}/exists");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();

            // Deserialize the JSON response into your ExistsResponse object
            var result = await response.Content.ReadFromJsonAsync<ExistsResponse>(_options);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize API response for game ID existence. Response body was null or invalid JSON for ExistsResponse type.");
            }

            return result.Exists;
        }

        public async Task<bool> IsGameIdInUseAsync(
        int gameId,
        ObservableCollection<Game> developerGames,
        ObservableCollection<Game> unvalidatedGames)
        {
            foreach (var game in developerGames)
            {
                if (game.GameId == gameId)
                {
                    return true;
                }
            }

            foreach (var game in unvalidatedGames)
            {
                if (game.GameId == gameId)
                {
                    return true;
                }
            }

            return await this.IsGameIdInUseAsync(gameId);
        }

        public async Task RejectGameAndRemoveFromUnvalidatedAsync(int gameId, ObservableCollection<Game> unvalidatedGames)
        {
            await this.RejectGameAsync(gameId);

            Game gameToRemove = null;
            foreach (var game in unvalidatedGames)
            {
                if (game.GameId == gameId)
                {
                    gameToRemove = game;
                    break;
                }
            }

            if (gameToRemove != null)
            {
                unvalidatedGames.Remove(gameToRemove);
            }
        }

        public async Task RejectGameAsync(int game_id)
        {
            try
            {

                var content = new StringContent(string.Empty);
                var response = await _httpClient.PatchAsync($"/api/Developer/games/{game_id}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error rejecting game: {ex.Message}", ex);
            }
        }


        public async Task RejectGameWithMessageAsync(int game_id, string message)
        {
            try
            {
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/api/Developer/games/{game_id}/reject-with-message", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error rejecting game with message: {ex.Message}", ex);
            }
        }


        public async Task UpdateGameAndRefreshListAsync(Game game, ObservableCollection<Game> developerGames)
        {
            Game existing = null;
            foreach (var gameInDeveloperGames in developerGames)
            {
                if (gameInDeveloperGames.GameId == game.GameId)
                {
                    existing = gameInDeveloperGames;
                    break;
                }
            }

            if (existing != null)
            {
                developerGames.Remove(existing);
            }

            await this.UpdateGameAsync(game);
            developerGames.Add(game);
        }

        public async Task UpdateGameAsync(Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var response = await _httpClient.PatchAsJsonAsync(
                "/api/Developer/games/update",
                game
            );

            response.EnsureSuccessStatusCode();
        }


        public async Task UpdateGameWithTagsAsync(Game game, IList<Tag> selectedTags)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var request = new UpdateGameWithTagsRequest
            {
                Game = game,
                SelectedTags = selectedTags.ToList()
            };

            var response = await _httpClient.PatchAsJsonAsync(
                "/api/Developer/games/update-with-tags",
                request
            );

            response.EnsureSuccessStatusCode();
        }

        public async Task ValidateGameAsync(int game_id)
        {
            try
            {
                var content = new StringContent(string.Empty);
                var response = await _httpClient.PatchAsync($"/api/Developer/games/validate/{game_id}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                throw new Exception($"Error validating item: {exception.Message}", exception);
            }
        }


        public Game ValidateInputForAddingAGame(
        string gameIdText,
        string name,
        string priceText,
        string description,
        string imageUrl,
        string trailerUrl,
        string gameplayUrl,
        string minimumRequirement,
        string reccommendedRequirement,
        string discountText,
        IList<Tag> selectedTags)
        {
            if (string.IsNullOrWhiteSpace(gameIdText) || string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(priceText) ||
                string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(imageUrl) ||
                string.IsNullOrWhiteSpace(minimumRequirement) ||
                string.IsNullOrWhiteSpace(reccommendedRequirement) || string.IsNullOrWhiteSpace(discountText))
            {
                throw new Exception(ExceptionMessages.AllFieldsRequired);
            }

            if (!int.TryParse(gameIdText, out int gameId))
            {
                throw new Exception(ExceptionMessages.InvalidGameId);
            }

            if (!decimal.TryParse(priceText, out var price) || price < ComparingValueForPositivePrice)
            {
                throw new Exception(ExceptionMessages.InvalidPrice);
            }

            if (!decimal.TryParse(discountText, out var discount) || discount < ComparingValueForMinimumDicount ||
                discount > ComparingValueForMaximumDicount)
            {
                throw new Exception(ExceptionMessages.InvalidDiscount);
            }

            if (selectedTags == null || selectedTags.Count == EmptyListLength)
            {
                throw new Exception(ExceptionMessages.NoTagsSelected);
            }

            var game = new Game
            {
                GameId = gameId,
                GameTitle = name,
                Price = price,
                GameDescription = description,
                ImagePath = imageUrl,
                GameplayPath = gameplayUrl,
                TrailerPath = trailerUrl,
                MinimumRequirements = minimumRequirement,
                RecommendedRequirements = reccommendedRequirement,
                Status = PendingState,
                Discount = discount,
                PublisherIdentifier = this.User.UserId,
            };
            return game;
        }

        public async Task DeleteGameAsync(int gameId)
        {
            var response = await _httpClient.DeleteAsync($"/api/Developer/games/{gameId}");
            response.EnsureSuccessStatusCode();
        }

        public IUserDetails GetCurrentUser()
        {
            return this.User;
        }

        public async Task CreateGameAsync(Game game, int userId)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var response = await _httpClient.PostAsJsonAsync(
                $"/api/games/{userId}/create",
                game
            );

            response.EnsureSuccessStatusCode();
        }


       
    }
}
