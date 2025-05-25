using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class ActiveTradesController : Controller
	{
		private readonly IUserService _userService;
		private readonly ITradeService _tradeService;

		public ActiveTradesController(IUserService userService, ITradeService tradeService)
		{
			_userService = userService;
			_tradeService = tradeService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var currentUser = _tradeService.GetCurrentUser();
			if (currentUser == null)
				return RedirectToAction("Login", "Account");

			var users = await _userService.GetAllUsersAsync();
			var activeTrades = await _tradeService.GetActiveTradesAsync(currentUser.UserId);

			var viewModel = new ActiveTradesViewModel
			{
				CurrentUserId = currentUser.UserId,
				Users = users.Select(user => new SelectListItem
				{
					Value = user.UserId.ToString(),
					Text = user.UserName
				}).ToList(),
				ActiveTrades = activeTrades.Select(trade => trade.ToTradeViewModelDetails()),
				CanAcceptOrDeclineTrade = true // Simplified assumption
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> LoadUserActiveTrades(ActiveTradesViewModel model)
		{
			var users = await _userService.GetAllUsersAsync();
			var trades = model.CurrentUserId.HasValue
				? await _tradeService.GetActiveTradesAsync(model.CurrentUserId.Value)
				: new List<ItemTrade>();

			model.Users = users.Select(user => new SelectListItem
			{
				Value = user.UserId.ToString(),
				Text = user.UserName
			}).ToList();

			model.ActiveTrades = trades.Select(trade => trade.ToTradeViewModelDetails());
			model.CanAcceptOrDeclineTrade = true;

			return View("Index", model);
		}

		[HttpPost]
		public async Task<IActionResult> RespondToTrade(int TradeId, string action)
		{
			var currentUser = _tradeService.GetCurrentUser();
			var trades = await _tradeService.GetActiveTradesAsync(currentUser.UserId);
			var trade = trades.FirstOrDefault(trade => trade.TradeId == TradeId);

			if (trade == null)
				return NotFound();

			if (action == "Accept")
			{
				bool isSourceUser = trade.SourceUser.UserId == currentUser.UserId;
				await _tradeService.AcceptTradeAsync(trade, isSourceUser);
			}
			else if (action == "Decline")
			{
				_tradeService.DeclineTradeRequest(trade); 
			}

			return RedirectToAction("Index", "ActiveTrades");
		}
	}
}
