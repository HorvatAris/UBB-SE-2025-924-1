namespace SteamHub.Api.Models.UserInventory
{
    public class UserInventoryResponse
    {
        public int UserId { get; set; }
        public required List<InventoryItemResponse> Items { get; set; }
    }

}