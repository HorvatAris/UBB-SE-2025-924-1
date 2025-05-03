namespace SteamHub.ApiContract.Models.UserInventory
{
    public class UserInventoryResponse
    {
        public int UserId { get; set; }
        public List<InventoryItemResponse> Items { get; set; }
    }
}