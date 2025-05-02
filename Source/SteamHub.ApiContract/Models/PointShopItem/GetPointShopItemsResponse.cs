using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.PointShopItem;

namespace SteamHub.Api.Models.PointShopItem;

public class GetPointShopItemsResponse
{
	public List<PointShopItemResponse> PointShopItems { get; set; }
}
