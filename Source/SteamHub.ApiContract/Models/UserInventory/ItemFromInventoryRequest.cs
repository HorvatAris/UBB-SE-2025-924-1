namespace SteamHub.ApiContract.Models.UserInventory
{
    public class ItemFromInventoryRequest
    {
        public int UserId { get; set; }

        public int ItemId { get; set; }

        public int GameId { get; set; }
    }
}