// <copyright file="IUserGameService.cs" company="PlaceholderCompany">
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

    public interface IUserGameService
    {
        int LastEarnedPoints { get; }

        void RemoveGameFromWishlist(Game game);

        void AddGameToWishlist(Game game);

        void PurchaseGames(List<Game> games);

        void ComputeNoOfUserGamesForEachTag(Collection<Tag> all_tags);

        Collection<Tag> GetFavoriteUserTags();

        void ComputeTagScoreForGames(Collection<Game> games);

        void ComputeTrendingScores(Collection<Game> games);

        Task<Collection<Game>> GetRecommendedGames();

        Collection<Game> GetWishListGames();

        Collection<Game> SearchWishListByName(string searchText);

        Collection<Game> FilterWishListGames(string criteria);

        bool IsGamePurchased(Game game);

        Collection<Game> SortWishListGames(string criteria, bool ascending);
    }
}
