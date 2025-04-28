namespace SteamHub.Api.Models.PointShopItem;

public class UpdatePointShopItemRequest
{
    public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public double PointPrice { get; set; }

    public string ItemType { get; set; }
}
