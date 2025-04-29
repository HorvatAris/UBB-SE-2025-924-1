namespace CtrlAltElite.Models
{
    using System;
    using System.Diagnostics;

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

        private Item()
        {
        }

        public int ItemId
        {
            get => this.itemId;
            set => this.itemId = value;
        }

        public string ItemName
        {
            get => this.itemName;
            set => this.itemName = value;
        }

        public Game Game
        {
            get => this.associatedGame;
            set => this.associatedGame = value;
        }

        public float Price
        {
            get => this.price;
            set => this.price = value;
        }

        public string Description
        {
            get => this.description;
            set => this.description = value;
        }

        public bool IsListed
        {
            get => this.isItemListed;
            set => this.isItemListed = value;
        }

        public string ImagePath
        {
            get => this.imagePath;
            set => this.imagePath = value;
        }

        public string GetItemName()
        {
            return this.itemName;
        }

        public Game GetCorrespondingGame()
        {
            return this.associatedGame;
        }

        public void SetItemDescription(string description)
        {
            this.description = description;
        }

        public void SetItemId(int id)
        {
            this.itemId = id;

            this.imagePath = this.GetDefaultImagePath(this.itemName);
            Debug.WriteLine($"Set ItemId {id} and image path: {this.imagePath}");
        }

        public void SetIsListed(bool isListed)
        {
            this.isItemListed = isListed;
        }

        public void SetImagePath(string imagePath)
        {
            Debug.WriteLine($"Setting image path for {this.itemName}: {imagePath}");
            this.imagePath = imagePath;
        }

        private string GetDefaultImagePath(string itemName)
        {
            string gameFolder = GameFolderResolver.GetFolderName(this.associatedGame.Name);
            var path = $"{ImageBasePath}{gameFolder}/{this.itemId}.png";
            Debug.WriteLine($"Generated image path for item {this.itemId} ({itemName}) from {this.associatedGame.Name}: {path}");
            return path;
        }

    }

}
