// <copyright file="IUserGameService.cs" company="PlaceholderCompany">
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

    public interface IUserGameService
    {
        int LastEarnedPoints { get; }

        Task RemoveGameFromWishlistAsync(Game game);

        Task AddGameToWishlistAsync(Game game);

        Task PurchaseGamesAsync(List<Game> games);

        Task ComputeNoOfUserGamesForEachTagAsync(Collection<Tag> all_tags);

        Task<Collection<Tag>> GetFavoriteUserTagsAsync();

        Task ComputeTagScoreForGamesAsync(Collection<Game> games);

        void ComputeTrendingScores(Collection<Game> games);

        Task<Collection<Game>> GetRecommendedGamesAsync();

        Task<Collection<Game>> GetWishListGamesAsync();

        Task<Collection<Game>> SearchWishListByNameAsync(string searchText);

        Task<Collection<Game>> FilterWishListGamesAsync(string criteria);

        Task<bool> IsGamePurchasedAsync(Game game);

        Task<Collection<Game>> SortWishListGamesAsync(string criteria, bool ascending);
    }
}
