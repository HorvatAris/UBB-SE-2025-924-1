﻿// <copyright file="IInventoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;

    public interface IInventoryService
    {
        Task<List<Item>> GetItemsFromInventoryAsync(Game game);

        Task<List<Item>> GetAllItemsFromInventoryAsync();

        Task AddItemToInventoryAsync(Game game, Item item);

        Task<List<Item>> GetUserInventoryAsync(int userId);

        IUserDetails GetAllUsers();

        Task<bool> SellItemAsync(Item item);

        List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText);

        Task<List<Game>> GetAvailableGamesAsync(List<Item> items);

        Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText);
    }
}