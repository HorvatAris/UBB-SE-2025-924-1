// <copyright file="IPointShopService.cs" company="PlaceholderCompany">
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
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.PointShopItem;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserPointShopItemInventory;

    public interface IPointShopService
    {
        IUserDetails GetCurrentUser();

        Task<List<PointShopItem>> GetAllItemsAsync();

        Task<Collection<PointShopItem>> GetUserItemsAsync(int userId);

        Task PurchaseItemAsync(PurchasePointShopItemRequest item);

        Task ActivateItemAsync(UpdateUserPointShopItemInventoryRequest request);

        Task DeactivateItemAsync(UpdateUserPointShopItemInventoryRequest request);

        Task<List<PointShopItem>> GetFilteredItemsAsync(string filterType, string searchText, double minimumPrice, double maximumPrice);

        bool CanUserPurchaseItem(IUserDetails user, PointShopItem selectedItem, IEnumerable<PointShopItem> userItems);

        Task<List<PointShopItem>> GetAvailableItemsAsync(IUserDetails user);

        bool TryPurchaseItem(PointShopItem selectedItem, ObservableCollection<PointShopTransaction> transactionHistory, IUserDetails user, out PointShopTransaction newTransaction);

        Task<PointShopItem> ToggleActivationForItemAsync(int itemId, ObservableCollection<PointShopItem> userItems);
    }
}
