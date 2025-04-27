using SteamHub.Api.Entities;

namespace SteamHub.Api.Models;

public class GetPointShopItemsResponse
{
	public List<PointShopItemResponse> PointShopItems { get; set; }
}
