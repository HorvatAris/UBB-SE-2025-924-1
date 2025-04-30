namespace SteamHub.Api.Models
{
    using Entities;

    public class UpdateItemTradeRequest
    {
        public string? TradeDescription { get; set; }
        public TradeStatusEnum? TradeStatus { get; set; }
        public bool? AcceptedBySourceUser { get; set; }
        public bool? AcceptedByDestinationUser { get; set; }
    }
}
