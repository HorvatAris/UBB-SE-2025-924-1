namespace SteamHub.Api.Entities;

public class PointShopItem
{
    public int PointShopItemId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public double PointPrice { get; set; }

    public string ItemType { get; set; }

    public IList<UserPointShopItemInventory> UserPointShopItemsInventory { get; set; }
}