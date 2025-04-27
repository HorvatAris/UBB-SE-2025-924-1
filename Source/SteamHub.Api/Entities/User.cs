namespace SteamHub.Api.Entities;

public class User
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public float WalletBalance { get; set; }

    public float PointsBalance { get; set; }

    public RoleEnum RoleId { get; set; }
    public Role UserRole { get; set; }

    public IList<UserPointShopItemInventory> UserPointShopItemsInventory { get; set; }
}