namespace SteamHub.ApiContract.Models.ItemTrade
{
    public class UpdateItemTradeRequest
    {
        public string? TradeDescription { get; set; }

        public TradeStatusEnum? TradeStatus { get; set; }

        public bool? AcceptedBySourceUser { get; set; }

        public bool? AcceptedByDestinationUser { get; set; }
    }
}
