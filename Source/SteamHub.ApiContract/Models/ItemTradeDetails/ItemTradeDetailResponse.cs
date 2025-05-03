namespace SteamHub.ApiContract.Models.ItemTradeDetails;
public class ItemTradeDetailResponse
{
    public int TradeId { get; set; }
    public int ItemId { get; set; }
    public bool IsSourceUserItem { get; set; }
}