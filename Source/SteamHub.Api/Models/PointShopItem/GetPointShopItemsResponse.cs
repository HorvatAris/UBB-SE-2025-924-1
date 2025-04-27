using SteamHub.Api.Entities;

namespace SteamHub.Api.Models.PointShopItem;

public class GetPointShopItemsResponse
{
	public List<PointShopItemResponse> PointShopItems { get; set; }
}
