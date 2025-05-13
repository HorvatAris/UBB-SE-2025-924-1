using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class TradeHistoryController : Controller
	{
		private readonly IUserService _userService;
		private readonly ITradeService _tradeService;

		public TradeHistoryController(IUserService userService, ITradeService tradeService)
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

			var allUsers = await _userService.GetAllUsersAsync();
			var history = await _tradeService.GetTradeHistoryAsync(currentUser.UserId);

			var viewModel = new TradeHistoryViewModel
			{
				CurrentUserId = currentUser.UserId,
				Users = allUsers.Select(user => new SelectListItem
				{
					Value = user.UserId.ToString(),
					Text = user.UserName
				}).ToList(),
				TradeHistory = history.Select(tradeHistory => tradeHistory.ToTradeHistoryViewModelDetails()).ToList()
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> LoadUserTradeHistory(TradeHistoryViewModel model)
		{
			var allUsers = await _userService.GetAllUsersAsync();

			var history = model.CurrentUserId.HasValue
				? await _tradeService.GetTradeHistoryAsync(model.CurrentUserId.Value)
				: new List<ApiContract.Models.ItemTrade.ItemTrade>();

			model.Users = allUsers.Select(user => new SelectListItem
			{
				Value = user.UserId.ToString(),
				Text = user.UserName
			}).ToList();

			model.TradeHistory = history.Select(tradeHistory => tradeHistory.ToTradeHistoryViewModelDetails()).ToList();

			return View("Index", model);
		}
	}
}
