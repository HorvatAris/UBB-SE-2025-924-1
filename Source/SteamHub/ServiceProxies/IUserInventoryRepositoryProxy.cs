// <copyright file="IUserInventoryServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ServiceProxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Refit;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamHub.ApiContract.Repositories;

    public interface IUserInventoryRepositoryProxy : IUserInventoryRepository
    {
        [Get("/api/UserInventory/{userId}")]
        Task<UserInventoryResponse> GetUserInventoryAsync(int userId);

        [Get("/api/UserInventory/{userId}/item/{itemId}")]
        Task<InventoryItemResponse?> GetItemFromUserInventoryAsync(int userId, int itemId);

        [Post("/api/UserInventory")]
        Task AddItemToUserInventoryAsync([Body] ItemFromInventoryRequest request);

        [Delete("/api/UserInventory")]
        Task RemoveItemFromUserInventoryAsync([Body] ItemFromInventoryRequest request);
    }
}
