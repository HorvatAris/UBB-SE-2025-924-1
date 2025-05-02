// <copyright file="IPointShopRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamStore.Models;

    public interface IPointShopRepository
    {
        List<PointShopItem> GetAllItems();

        User GetCurrentUser();

        List<PointShopItem> GetUserItems();

        void PurchaseItem(PointShopItem item);

        void ActivateItem(PointShopItem item);

        void DeactivateItem(PointShopItem item);

        void UpdateUserPointBalance();

        void ResetUserInventory();
    }
}
