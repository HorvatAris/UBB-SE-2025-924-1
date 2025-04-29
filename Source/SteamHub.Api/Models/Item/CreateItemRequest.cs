namespace SteamHub.Api.Models.Item
{
    public class CreateItemRequest
    {
        public string ItemName { get; set; } = default!;
        public int GameId { get; set; }
        public float Price { get; set; }
        public string Description { get; set; } = default!;
        public string ImagePath { get; set; } = default!;
    }
}
