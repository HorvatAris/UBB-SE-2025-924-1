// <copyright file="Item.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Models.Item
{
    using System;
    using System.Diagnostics;
    using SteamHub.ApiContract.Models.Game;

    public class Item
    {
        public Item()
        {
        }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public Game Game { get; set; }

        public string GameName { get; set; } = default!;

        public float Price { get; set; }

        public string Description { get; set; }

        public bool IsListed { get; set; }

        public string ImagePath { get; set; }
    }
}
