// <copyright file="IPointShopService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.Models;
    using SteamHub.Models;

    public interface IPointShopService
    {
        User GetCurrentUser();

        Task<List<PointShopItem>> GetAllItemsAsync();

        Task<Collection<PointShopItem>> GetUserItemsAsync();

        Task PurchaseItemAsync(PointShopItem item);

        Task ActivateItemAsync(PointShopItem item);

        Task DeactivateItemAsync(PointShopItem item);

        Task<List<PointShopItem>> GetFilteredItemsAsync(string filterType, string searchText, double minimumPrice, double maximumPrice);

        bool CanUserPurchaseItem(User user, PointShopItem selectedItem, IEnumerable<PointShopItem> userItems);

        Task<List<PointShopItem>> GetAvailableItemsAsync(User user);

        bool TryPurchaseItem(PointShopItem selectedItem, ObservableCollection<PointShopTransaction> transactionHistory, User user, out PointShopTransaction newTransaction);

        Task<PointShopItem> ToggleActivationForItemAsync(int itemId, ObservableCollection<PointShopItem> userItems);
    }
}
