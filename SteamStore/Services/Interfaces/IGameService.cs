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
        Collection<Game> GetAllGames();

        Collection<Tag> GetAllTags();

        Collection<Tag> GetAllGameTags(Game game);

        Collection<Game> SearchGames(string search_query);

        Collection<Game> FilterGames(int minimumRating, int minimumPrice, int maximumPrice, string[] tags);

        void ComputeTrendingScores(Collection<Game> games);

        Collection<Game> GetTrendingGames();

        Collection<Game> GetDiscountedGames();

        List<Game> GetSimilarGames(int gameId);

        Game GetGameById(int gameId);
    }
}
