using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Models.ItemTrade;

namespace SteamHub.Web.ViewModels
{
	public class TradeHistoryViewModel
	{
		public int? CurrentUserId { get; set; } // For the current user selection
		public List<SelectListItem> Users { get; set; } // List of users to populate the select dropdown

		public IEnumerable<TradeHistoryViewModelDetails> TradeHistory { get; set; } // List of trade history details
	}
}
