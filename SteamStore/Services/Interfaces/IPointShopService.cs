// <copyright file="IPointShopService.cs" company="PlaceholderCompany">
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

    public interface IPointShopService
    {
        User GetCurrentUser();

        List<PointShopItem> GetAllItems();

        List<PointShopItem> GetUserItems();

        void PurchaseItem(PointShopItem item);

        void ActivateItem(PointShopItem item);

        void DeactivateItem(PointShopItem item);

        List<PointShopItem> GetFilteredItems(string filterType, string searchText, double minimumPrice, double maximumPrice);

        bool CanUserPurchaseItem(User user, PointShopItem selectedItem, IEnumerable<PointShopItem> userItems);

        List<PointShopItem> GetAvailableItems(User user);

        bool TryPurchaseItem(PointShopItem selectedItem, ObservableCollection<PointShopTransaction> transactionHistory, User user, out PointShopTransaction newTransaction);

        PointShopItem ToggleActivationForItem(int itemId, ObservableCollection<PointShopItem> userItems);
    }
}
