namespace SteamHub.Api.Models.Item
{
    public class UpdateItemRequestDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = default!;
        public float Price { get; set; }
        public string Description { get; set; } = default!;
        public bool IsListed { get; set; }
        public string ImagePath { get; set; } = default!;
    }
}
