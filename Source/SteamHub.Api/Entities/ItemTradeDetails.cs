namespace SteamHub.Api.Entities;

public class ItemTradeDetail
{
    public int TradeId { get; set; }
    public int ItemId { get; set; }
    public bool IsSourceUserItem { get; set; }

    public ItemTrade ItemTrade { get; set; }
    public Item Item { get; set; }
}
