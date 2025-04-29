namespace SteamHub.Api.Models.Item
{
    public class CreateItemRequest
    {
        // The client provides the item name.
        public string ItemName { get; set; } = default!;

        // The Game Id is used to look up the associated game.
        public int GameId { get; set; }

        // The price for the item.
        public float Price { get; set; }

        // A description for the item.
        public string Description { get; set; } = default!;
    }
}
