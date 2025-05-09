// <copyright file="PointShopItemMapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using SteamHub.ApiContract.Models.PointShopItem;

namespace SteamHub.ApiContract.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.PointShopItem;

    public class PointShopItemMapper
    {
        public static PointShopItem MapToPointShopItem(PointShopItemResponse item)
        {
            return new PointShopItem
            {
                ItemIdentifier = item.PointShopItemId,
                Name = item.Name,
                Description = item.Description,
                ImagePath = item.ImagePath,
                PointPrice = item.PointPrice,
                ItemType = item.ItemType,
            };
        }
    }
}
