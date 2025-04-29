// <copyright file="IDeveloperService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamStore.Models;

    public interface IDeveloperService
    {
        Task ValidateGame(int game_id);

        Game ValidateInputForAddingAGame(string gameIdText, string name, string priceText, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string reccommendedRequirement, string discountText, IList<Tag> selectedTags);

        Task<Game> CreateValidatedGame(
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
            IList<Tag> selectedTags);

        Game FindGameInObservableCollectionById(int gameId, ObservableCollection<Game> gameList);

        Task CreateGame(Game game);

        Task CreateGameWithTags(Game game, IList<Tag> selectedTags);

        Task UpdateGame(Game game);

        Task UpdateGameWithTags(Game game, IList<Tag> selectedTags);

        Task DeleteGame(int game_id);

        Task<List<Game>> GetDeveloperGames();

        Task<List<Game>> GetUnvalidated();

        Task RejectGame(int game_id);

        Task RejectGameWithMessage(int game_id, string message);

        Task<string> GetRejectionMessage(int game_id);

        Task InsertGameTag(int gameId, int tagId);

        Collection<Tag> GetAllTags();

        Task<bool> IsGameIdInUse(int gameId);

        Task<List<Tag>> GetGameTags(int gameId);

        Task DeleteGameTags(int gameId);

        int GetGameOwnerCount(int game_id);

        User GetCurrentUser();

        Task DeleteGame(int gameId, ObservableCollection<Game> developerGames);

        Task UpdateGameAndRefreshList(Game game, ObservableCollection<Game> developerGames);

        Task RejectGameAndRemoveFromUnvalidated(int gameId, ObservableCollection<Game> unvalidatedGames);

        Task<bool> IsGameIdInUse(
            int gameId,
            ObservableCollection<Game> developerGames,
            ObservableCollection<Game> unvalidatedGames);

        Task<IList<Tag>> GetMatchingTagsForGame(int gameId, IList<Tag> allAvailableTags);
    }
}
