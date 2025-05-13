using SteamHub.ApiContract.Models.ItemTrade;

namespace SteamHub.Web.ViewModels
{
	public static class MappingExtensions
	{
		public static TradeViewModelDetails ToTradeViewModelDetails(this ItemTrade trade)
		{
			return new TradeViewModelDetails
			{
				Id = trade.TradeId,
				TradeDescription = trade.TradeDescription,
				TradeStatus = trade.TradeStatus,
				SourceUser = new ActiveTradesUserViewModel
				{
					UserName = trade.SourceUser?.UserName,
					UserId = trade.SourceUser.UserId
				},
				DestinationUser = new ActiveTradesUserViewModel
				{
					UserName = trade.DestinationUser?.UserName,
					UserId = trade.DestinationUser.UserId
				},
				SourceUserItems = trade.SourceUserItems?
					.Select(item => new ActiveTradesItemViewModel
					{
						ItemName = item.ItemName
					}).ToList() ?? new(),
				DestinationUserItems = trade.DestinationUserItems?
					.Select(item => new ActiveTradesItemViewModel
					{
						ItemName = item.ItemName
					}).ToList() ?? new()
			};
		}

		public static TradeHistoryViewModelDetails ToTradeHistoryViewModelDetails(this ItemTrade trade)
		{
			return new TradeHistoryViewModelDetails
			{
				TradeDescription = trade.TradeDescription,
				TradeStatus = trade.TradeStatus,
				SourceUser = new TradeHistoryUserViewModel
				{
					UserName = trade.SourceUser?.UserName
				},
				DestinationUser = new TradeHistoryUserViewModel
				{
					UserName = trade.DestinationUser?.UserName
				},
				SourceUserItems = trade.SourceUserItems?
					.Select(item => new TradeHistoryItemViewModel
					{
						ItemName = item.ItemName
					}).ToList() ?? new(),
				DestinationUserItems = trade.DestinationUserItems?
					.Select(item => new TradeHistoryItemViewModel
					{
						ItemName = item.ItemName
					}).ToList() ?? new()
			};
		}
	}
}
