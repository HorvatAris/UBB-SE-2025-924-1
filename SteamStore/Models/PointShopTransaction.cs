﻿// <copyright file="PointShopTransaction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PointShopTransaction
    {
        public PointShopTransaction(int identifier, string itemName, double pointsSpent, string itemType)
        {
            this.Identifier = identifier;
            this.ItemName = itemName;
            this.PointsSpent = pointsSpent;
            this.PurchaseDate = DateTime.Now;
            this.ItemType = itemType;
        }

        public int Identifier { get; set; }

        public string ItemName { get; set; }

        public double PointsSpent { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string ItemType { get; set; }
    }
}
