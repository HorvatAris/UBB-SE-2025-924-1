namespace SteamHub.Api.Entities
{
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
        public virtual int CorrespondingGameId { get; set; } = default!;

        public float Price { get; set; }

        [Required]
        public string Description { get; set; } = default!;

        public bool IsListed { get; set; }

        public string ImagePath { get; set; } = default!;

        public IList<ItemTradeDetail> ItemTradeDetails { get; set; }


        public Item() { }

        public Item(string itemName, int correspondingGameId, float price, string description)
        {
            ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            CorrespondingGameId = correspondingGameId;
            Price = price;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            IsListed = false;
        }
    }
}
