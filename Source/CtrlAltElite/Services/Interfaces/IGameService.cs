// <copyright file="IGameService.cs" company="PlaceholderCompany">
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

    public interface IGameService
    {
        Task<Collection<Game>> GetAllGames();

        Collection<Tag> GetAllTags();

        Collection<Tag> GetAllGameTags(Game game);

        Task<Collection<Game>> SearchGames(string search_query);

        Task<Collection<Game>> FilterGames(int minimumRating, int minimumPrice, int maximumPrice, string[] tags);

        void ComputeTrendingScores(Collection<Game> games);

        Task<Collection<Game>> GetTrendingGames();

        Task<Collection<Game>> GetDiscountedGames();

        Task<List<Game>> GetSimilarGames(int gameId);

        Task<Game> GetGameById(int gameId);
    }
}
