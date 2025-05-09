// <copyright file="PointShopItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Models.PointShopItem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PointShopItem
    {
        public PointShopItem(int itemIdentifier, string name, string description, string imagePath, double pointPrice, string itemType)
        {
            ItemIdentifier = itemIdentifier;
            Name = name;
            Description = description;
            ImagePath = imagePath;
            PointPrice = pointPrice;
            ItemType = itemType;
            IsActive = false;
        }

        public PointShopItem()
        {
        }

        public int ItemIdentifier { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public double PointPrice { get; set; }

        public string ItemType { get; set; } // E.g., "ProfileBackground", "Avatar", "Emoticon", etc.

        public bool IsActive { get; set; }
    }
}