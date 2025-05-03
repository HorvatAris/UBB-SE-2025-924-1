namespace SteamHub.ApiContract.Models.ItemTradeDetails;
public class CreateItemTradeDetailRequest
{
    public int TradeId { get; set; }
    public int ItemId { get; set; }
    public bool IsSourceUserItem { get; set; }
}
