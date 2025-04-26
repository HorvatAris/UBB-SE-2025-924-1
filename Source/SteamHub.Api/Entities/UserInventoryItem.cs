using System.ComponentModel.DataAnnotations.Schema;

namespace SteamHub.Api.Entities;

public class UserInventoryItem
{
	public UserInventoryItem()
	{

	}

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("PointShopItem")]
    public int ItemIdentifier { get; set; }
    public PointShopItem PointShopItem { get; set; }

    public DateTime PurchaseDate { get; set; }
	public bool isActive { get; set; }

}
