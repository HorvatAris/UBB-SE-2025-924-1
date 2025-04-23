// <copyright file="IUserGameRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IUserGameRepository
    {
        bool IsGamePurchased(Game game);

        void RemoveGameFromWishlist(Game game);

        void AddGameToPurchased(Game game);

        void AddGameToWishlist(Game game);

        string[] GetGameTags(int gameId);

        Collection<Game> GetAllUserGames();

        void AddPointsForPurchase(float purchaseAmount);

        float GetUserPointsBalance();

        Collection<Game> GetWishlistGames();

        int GetGameOwnerCount(int gameIdentifier);
    }
}
