namespace SteamHub.Api.Models.UserInventory
{
    public class InventoryItemResponse
    {
        public int ItemId { get; set; }
        public required string ItemName { get; set; } 
        public float Price { get; set; }
        public required string Description { get; set; } 
        public bool IsListed { get; set; }
        public required string GameName { get; set; }
        public required string ImagePath { get; set; }
    }

}