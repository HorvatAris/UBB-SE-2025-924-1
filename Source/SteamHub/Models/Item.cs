namespace SteamHub.Models
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents an item that can be listed and traded in the game system.
    /// </summary>
    public class Item
    {
        public const string GameTitleCounterStrike = "counter-strike 2";
        public const string GameTitleDota = "dota 2";
        public const string GameTitleTeamFortress = "team fortress 2";

        public const string GameFolderCounterStrike = "cs2";
        public const string GameFolderDota = "dota2";
        public const string GameFolderTeamFortress = "tf2";

        public int ItemId { get; set; }
        public string ItemName { get; set; } = default!;
        public Game Game { get; set; } = default!;
        public float Price { get; set; }
        public string Description { get; set; } = default!;
        public bool isListed { get; set; }
        public string ImagePath { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class for EF Core.
        /// </summary>
        private Item()
        {
        }
    }
}