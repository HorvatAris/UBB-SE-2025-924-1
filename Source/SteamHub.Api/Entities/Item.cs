namespace SteamHub.Api.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = default!;

        public int CorrespondingGameId { get; set; }

        public virtual Game Game { get; set; } = default!;

        public float Price { get; set; }
        public string Description { get; set; } = default!;
        public bool IsListed { get; set; }
        public string ImagePath { get; set; } = default!;
    }
}
