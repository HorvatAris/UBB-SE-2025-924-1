// <copyright file="Item.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Domain.Entities
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents an item that can be listed and traded in the game system.
    /// </summary>
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
        private Game associatedGame = default!;
        private float price;
        private string description = default!;
        private bool isItemListed;
        private string imagePath = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class with provided values.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="game">The game to which the item belongs.</param>
        /// <param name="price">The price of the item.</param>
        /// <param name="description">The description of the item.</param>
        public Item(string itemName, Game game, float price, string description)
        {
            itemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            game = game ?? throw new ArgumentNullException(nameof(game));
            description = description ?? throw new ArgumentNullException(nameof(description));
            this.itemName = itemName;
            this.associatedGame = game;
            this.price = price;
            this.isItemListed = false;
            this.description = description;

            Debug.WriteLine($"Created item {itemName}, waiting for ItemId to set image path");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class for EF Core.
        /// </summary>
        private Item()
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier of the item.
        /// </summary>
        public int ItemId
        {
            get => this.itemId;
            set => this.itemId = value;
        }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string ItemName
        {
            get => this.itemName;
            set => this.itemName = value;
        }

        /// <summary>
        /// Gets or sets the game associated with the item.
        /// </summary>
        public Game Game
        {
            get => this.associatedGame;
            set => this.associatedGame = value;
        }

        /// <summary>
        /// Gets or sets the price of the item.
        /// </summary>
        public float Price
        {
            get => this.price;
            set => this.price = value;
        }

        /// <summary>
        /// Gets the price of the item as a formatted string.
        /// </summary>
        public string PriceDisplay
        {
            get => $"${this.price:F2}";
        }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string Description
        {
            get => this.description;
            set => this.description = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is listed.
        /// </summary>
        public bool IsListed
        {
            get => this.isItemListed;
            set => this.isItemListed = value;
        }

        /// <summary>
        /// Gets or sets the image path for the item.
        /// </summary>
        public string ImagePath
        {
            get => this.imagePath;
            set => this.imagePath = value;
        }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        /// <returns>The item name.</returns>
        public string GetItemName()
        {
            return this.itemName;
        }

        /// <summary>
        /// Gets the game associated with the item.
        /// </summary>
        /// <returns>The corresponding game.</returns>
        public Game GetCorrespondingGame()
        {
            return this.associatedGame;
        }

        /// <summary>
        /// Sets the item description.
        /// </summary>
        /// <param name="description">The new description.</param>
        public void SetItemDescription(string description)
        {
            this.description = description;
        }

        /// <summary>
        /// Sets the unique identifier of the item and generates its image path.
        /// </summary>
        /// <param name="id">The identifier to assign.</param>
        public void SetItemId(int id)
        {
            this.itemId = id;

            this.imagePath = this.GetDefaultImagePath(this.itemName);
            Debug.WriteLine($"Set ItemId {id} and image path: {this.imagePath}");
        }

        /// <summary>
        /// Sets whether the item is listed.
        /// </summary>
        /// <param name="isListed">True if listed; otherwise, false.</param>
        public void SetIsListed(bool isListed)
        {
            this.isItemListed = isListed;
        }

        /// <summary>
        /// Sets the price of the item.
        /// </summary>
        /// <param name="price">The price to assign.</param>
        public void SetPrice(float price)
        {
            this.price = price;
        }

        /// <summary>
        /// Sets the image path for the item.
        /// </summary>
        /// <param name="imagePath">The image path to assign.</param>
        public void SetImagePath(string imagePath)
        {
            Debug.WriteLine($"Setting image path for {this.itemName}: {imagePath}");
            this.imagePath = imagePath;
        }

        /// <summary>
        /// Generates a default image path for the item based on the game title and item ID.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <returns>The generated image path.</returns>
        private string GetDefaultImagePath(string itemName)
        {
            // Get the game folder name based on the game title
            string gameFolder = this.associatedGame.GameTitle.ToLower() switch
            {
                GameTitleCounterStrike => GameFolderCounterStrike,
                GameTitleDota => GameFolderDota,
                GameTitleTeamFortress => GameFolderTeamFortress,
                _ => this.associatedGame.GameTitle.ToLower().Replace(" ", string.Empty).Replace(":", string.Empty)
            };

            var path = $"{ImageBasePath}{gameFolder}/{this.itemId}.png";
            Debug.WriteLine($"Generated image path for item {this.itemId} ({itemName}) from {this.associatedGame.GameTitle}: {path}");
            return path;
        }
    }
}