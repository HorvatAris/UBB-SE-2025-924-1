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
        void ValidateGame(int game_id);

        Game ValidateInputForAddingAGame(string gameIdText, string name, string priceText, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string reccommendedRequirement, string discountText, IList<Tag> selectedTags);

        Game CreateValidatedGame(string gameIdText, string name, string priceText, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string reccommendedRequirement, string discountText, IList<Tag> selectedTags);

        Game FindGameInObservableCollectionById(int gameId, ObservableCollection<Game> gameList);

        void CreateGame(Game game);

        void CreateGameWithTags(Game game, IList<Tag> selectedTags);

        void UpdateGame(Game game);

        void UpdateGameWithTags(Game game, IList<Tag> selectedTags);

        void DeleteGame(int game_id);

        List<Game> GetDeveloperGames();

        List<Game> GetUnvalidated();

        void RejectGame(int game_id);

        void RejectGameWithMessage(int game_id, string message);

        string GetRejectionMessage(int game_id);

        void InsertGameTag(int gameId, int tagId);

        Collection<Tag> GetAllTags();

        bool IsGameIdInUse(int gameId);

        List<Tag> GetGameTags(int gameId);

        void DeleteGameTags(int gameId);

        int GetGameOwnerCount(int game_id);

        User GetCurrentUser();

        void DeleteGame(int gameId, ObservableCollection<Game> developerGames);

        void UpdateGameAndRefreshList(Game game, ObservableCollection<Game> developerGames);

        void RejectGameAndRemoveFromUnvalidated(int gameId, ObservableCollection<Game> unvalidatedGames);

        bool IsGameIdInUse(int gameId, ObservableCollection<Game> developerGames, ObservableCollection<Game> unvalidatedGames);

        IList<Tag> GetMatchingTagsForGame(int gameId, IList<Tag> allAvailableTags);
    }
}
