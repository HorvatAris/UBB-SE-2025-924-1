namespace SteamHub.Api.Entities
{
    using SteamHub.Api.Models.Game;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public string ItemName { get; set; } = default!;

        [Required]
        public virtual Game Game { get; set; } = default!;

        public float Price { get; set; }

        [Required]
        public string Description { get; set; } = default!;

        public bool IsListed { get; set; }

        public string ImagePath { get; set; } = default!;

        public Item() { }

        public Item(string itemName, Game game, float price, string description)
        {
            ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            Game = game ?? throw new ArgumentNullException(nameof(game));
            Price = price;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            IsListed = false;
        }
    }
}
