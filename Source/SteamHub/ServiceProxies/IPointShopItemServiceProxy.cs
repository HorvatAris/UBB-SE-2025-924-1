// <copyright file="IPointShopItemServiceProxy.cs" company="PlaceholderCompany">
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
    using SteamHub.ApiContract.Models.PointShopItem;
    using SteamHub.ApiContract.Repositories;

    public interface IPointShopItemServiceProxy : IPointShopItemRepository
    {
        [Get("/api/PointShopItems")]
        Task<GetPointShopItemsResponse> GetPointShopItemsAsync();
    }
}
