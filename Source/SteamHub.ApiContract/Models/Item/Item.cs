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
        private const string ImageBasePath = "ms-appx:///Assets/img/games/";
        private const string GameTitleCounterStrike = "counter-strike 2";
        private const string GameTitleDota = "dota 2";
        private const string GameTitleTeamFortress = "team fortress 2";

        private const string GameFolderCounterStrike = "cs2";
        private const string GameFolderDota = "dota2";
        private const string GameFolderTeamFortress = "tf2";

        private int itemId;
        private string itemName = default!;
        public Game Game { get; set; }
        private float price;
        private string description = default!;
        private bool isItemListed;
        private string imagePath = default!;

        public Item(string itemName, Game game, float price, string description)
        {
            itemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            game = game ?? throw new ArgumentNullException(nameof(game));
            description = description ?? throw new ArgumentNullException(nameof(description));
            this.itemName = itemName;
             this.Game= game;
            this.GameName = game.GameTitle;
            this.price = price;
            isItemListed = false;
            this.description = description;

            Debug.WriteLine($"Created item {itemName}, waiting for ItemId to set image path");
        }

        public Item()
        {
        }

        public int ItemId
        {
            get => itemId;
            set => itemId = value;
        }

        public string ItemName
        {
            get => itemName;
            set => itemName = value;
        }

        

        public float Price
        {
            get => price;
            set => price = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public bool IsListed
        {
            get => isItemListed;
            set => isItemListed = value;
        }

        public string ImagePath
        {
            get => imagePath;
            set => imagePath = value;
        }

        public string GameName { get; internal set; }

        public string GetItemName()
        {
            return itemName;
        }

        public Game GetCorrespondingGame()
        {
            return this.Game;
        }

        public void SetItemDescription(string description)
        {
            this.description = description;
        }

        public void SetItemId(int id)
        {
            itemId = id;

            // this.imagePath = this.GetDefaultImagePath(this.itemName);
            Debug.WriteLine($"Set ItemId {id} and image path: {imagePath}");
        }

        public void SetIsListed(bool isListed)
        {
            isItemListed = isListed;
        }

        public void SetImagePath(string imagePath)
        {
            Debug.WriteLine($"Setting image path for {itemName}: {imagePath}");
            this.imagePath = imagePath;
        }

        private string GetDefaultImagePath(string itemName)
        {
            //string gameFolder = GameFolderResolver.GetFolderName(associatedGame.GameTitle);
            //var path = $"{ImageBasePath}{gameFolder}/{itemId}.png";
            //Debug.WriteLine($"Generated image path for item {itemId} ({itemName}) from {associatedGame.GameTitle}: {path}");
            //return path;
            return String.Empty;
        }
    }
}
