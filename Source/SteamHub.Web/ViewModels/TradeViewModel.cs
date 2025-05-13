using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Models.Item;
using System.Collections.Generic;

namespace SteamHub.Web.ViewModels
{
	public class TradeViewModel
	{
		// Dropdowns
		public int? CurrentUserId { get; set; }
		public int? SelectedUserId { get; set; }
		public int? SelectedGameId { get; set; }

		public string TradeDescription { get; set; }

		public List<SelectListItem> Users { get; set; } = new();
		public List<SelectListItem> AvailableUsers { get; set; } = new();
		public List<SelectListItem> Games { get; set; } = new();

		// Inventory
		public List<Item> SourceUserItems { get; set; } = new();
		public List<Item> DestinationUserItems { get; set; } = new();

		// Selected Item Ids (from checkboxes)
		public List<int> SelectedSourceItemIds { get; set; } = new();
		public List<int> SelectedDestinationItemIds { get; set; } = new();

		// Selected items (those already added to trade)
		public List<Item> SelectedSourceItems { get; set; } = new();
		public List<Item> SelectedDestinationItems { get; set; } = new();

		// UI Feedback
		public string ErrorMessage { get; set; }
		public string SuccessMessage { get; set; }
	}
}
