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
        Task<Collection<Game>> GetAllGamesAsync();

        Task<Collection<Tag>> GetAllTagsAsync();

        Task<Collection<Game>> GetAllApprovedGamesAsync();

        Task<Collection<Tag>> GetAllGameTagsAsync(Game game);

        Task<Collection<Game>> SearchGamesAsync(string search_query);

        Task<Collection<Game>> FilterGamesAsync(int minimumRating, int minimumPrice, int maximumPrice, string[] tags);

        void ComputeTrendingScores(Collection<Game> games);

        Task<Collection<Game>> GetTrendingGamesAsync();

        Task<Collection<Game>> GetDiscountedGamesAsync();

        Task<List<Game>> GetSimilarGamesAsync(int gameId);

        Task<Game> GetGameByIdAsync(int gameId);
    }
}
