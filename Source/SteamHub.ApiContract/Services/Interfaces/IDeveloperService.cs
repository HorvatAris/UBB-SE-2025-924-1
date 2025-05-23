// <copyright file="IDeveloperService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Models.User;

    public interface IDeveloperService
    {
        IUserDetails GetCurrentUser();
        Task ValidateGameAsync(int game_id);

        Game ValidateInputForAddingAGame(string gameIdText, string name, string priceText, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string reccommendedRequirement, string discountText, IList<Tag> selectedTags);

        Task<Game> CreateValidatedGameAsync(
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
            IList<Tag> selectedTags,int userId);

        Game FindGameInObservableCollectionById(int gameId, ObservableCollection<Game> gameList);

        Task CreateGameAsync(Game game, int userId);

        Task CreateGameWithTagsAsync(Game game, IList<Tag> selectedTags,int userId);

        Task UpdateGameAsync(Game game);

        Task UpdateGameWithTagsAsync(Game game, IList<Tag> selectedTags);

        Task DeleteGameAsync(int gameId, ObservableCollection<Game> developerGames);
        Task DeleteGameAsync(int game_id);

        Task<List<Game>> GetDeveloperGamesAsync();

        Task<List<Game>> GetUnvalidatedAsync();

        Task RejectGameAsync(int game_id);

        Task RejectGameWithMessageAsync(int game_id, string message);

        Task<string> GetRejectionMessageAsync(int game_id);

        Task InsertGameTagAsync(int gameId, int tagId);

        Task<Collection<Tag>> GetAllTagsAsync();

        Task<bool> IsGameIdInUseAsync(int gameId);

        Task<List<Tag>> GetGameTagsAsync(int gameId);

        Task DeleteGameTagsAsync(int gameId);

        Task<int> GetGameOwnerCountAsync(int gameId);


        Task UpdateGameAndRefreshListAsync(Game game, ObservableCollection<Game> developerGames);

        Task RejectGameAndRemoveFromUnvalidatedAsync(int gameId, ObservableCollection<Game> unvalidatedGames);

        Task<bool> IsGameIdInUseAsync(
            int gameId,
            ObservableCollection<Game> developerGames,
            ObservableCollection<Game> unvalidatedGames);

        Task<IList<Tag>> GetMatchingTagsForGameAsync(int gameId, IList<Tag> allAvailableTags);
    }
}
