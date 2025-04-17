// <copyright file="IGameRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using SteamStore.Models;

    public interface IGameRepository
    {
        void CreateGame(Game game);

        Collection<Game> GetAllGames();

        List<Game> GetUnvalidated(int userUserIdentifier);

        void DeleteGameTags(int gameIdentifier);

        List<Game> GetDeveloperGames(int userUserIdentifier);

        void UpdateGame(int gameIdentifier, Game game);

        void RejectGame(int gameIdentifier);

        void RejectGameWithMessage(int gameIdentifier, string message);

        string GetRejectionMessage(int gameIdentifier);

        void InsertGameTag(int gameIdentifier, int tagIdentifier);

        bool IsGameIdInUse(int gameIdentifier);

        List<Tag> GetGameTags(int gameIdentifier);

        void ValidateGame(int gameIdentifier);

        void DeleteGame(int gameIdentifier);
    }
}