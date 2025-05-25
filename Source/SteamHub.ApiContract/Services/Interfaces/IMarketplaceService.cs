// <copyright file="IMarketplaceService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;

    public interface IMarketplaceService
    {
        IUserDetails User { get; set; }

        Task AddListingAsync(Game game, Item item);

        Task<bool> BuyItemAsync(Item item, int userId);

        Task<List<Item>> GetAllListingsAsync();

        Task<List<User>> GetAllUsersAsync();

        Task<List<Item>> GetListingsByGameAsync(int gameId, int userId);

        Task RemoveListingAsync(Game game, Item item);

        Task UpdateListingAsync(int gameId, int itemId);

        Task SwitchListingStatusAsync(int gameId, int itemId);
    }
}