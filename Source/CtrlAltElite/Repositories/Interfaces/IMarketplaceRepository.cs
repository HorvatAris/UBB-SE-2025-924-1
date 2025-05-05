// <copyright file="IMarketplaceRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using SteamStore.Models;

    public interface IMarketplaceRepository
    {
        Task<bool> BuyItemAsync(Item item, User currentUser);

        Task<List<Item>> GetAllListedItemsAsync();

        Task<List<Item>> GetListedItemsByGameAsync(Game game);

        Task MakeItemListableAsync(Game game, Item item);

        Task MakeItemNotListableAsync(Game game, Item item);

        Task UpdateItemPriceAsync(Game game, Item item);

        User? GetCurrentUser();

        Task<List<User>> GetAllUsersAsync();
    }
}