// <copyright file="IInventoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using SteamStore.Models;

    public interface IInventoryRepository
    {
        Task<List<Item>> GetItemsFromInventoryAsync(Game game);

        Task<List<Item>> GetUserInventoryAsync(int userId);

        Task<List<Item>> GetAllItemsFromInventoryAsync(User user);

        Task AddItemToInventoryAsync(Game game, Item item, User user);

        Task RemoveItemFromInventoryAsync(Game game, Item item, User user);

        Task<List<User>> GetAllUsersAsync();

        Task<bool> SellItemAsync(Item item);
    }
}