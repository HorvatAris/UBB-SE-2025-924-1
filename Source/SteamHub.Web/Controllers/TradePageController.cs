using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
	public class TradePageController : Controller
	{
		private readonly IUserService _userService;
		private readonly IGameService _gameService;
		private readonly ITradeService _tradeService;

		public TradePageController(IUserService userService, IGameService gameService, ITradeService tradeService)
		{
			_userService = userService;
			_gameService = gameService;
			_tradeService = tradeService;
		}

		public async Task<IActionResult> Index()
		{
			var currentUser = _tradeService.GetCurrentUser();
			if (currentUser == null)
				return RedirectToAction("Login", "Account");

			var allUsers = await _userService.GetAllUsersAsync();
			var games = await _gameService.GetAllGamesAsync();

			var viewModel = new TradeViewModel
			{
				CurrentUserId = currentUser.UserId,
				Users = allUsers.Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.UserName }).ToList(),
				AvailableUsers = allUsers.Where(u => u.UserId != currentUser.UserId)
										 .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.UserName }).ToList(),
				Games = games.Select(g => new SelectListItem { Value = g.GameId.ToString(), Text = g.GameTitle }).ToList(),
				SourceUserItems = await _tradeService.GetUserInventoryAsync(currentUser.UserId),
			};

			return View(viewModel);
		}

		public async Task<IActionResult> CreateTradeOffer(TradeViewModel model)
		{
			var currentUser = _tradeService.GetCurrentUser();
			if (currentUser == null)
			{
				model.ErrorMessage = "You must be logged in.";
				return View("Index", await RebuildModel(model));
			}

			if (model.CurrentUserId == null || model.SelectedUserId == null)
			{
				model.ErrorMessage = "Both users must be selected.";
				return View("Index", await RebuildModel(model));
			}

			if (!model.SelectedSourceItemIds.Any() && !model.SelectedDestinationItemIds.Any())
			{
				model.ErrorMessage = "Select at least one item to trade.";
				return View("Index", await RebuildModel(model));
			}

			var sourceItems = await _tradeService.GetUserInventoryAsync(model.CurrentUserId.Value);
			var destinationItems = await _tradeService.GetUserInventoryAsync(model.SelectedUserId.Value);

			var selectedSourceItems = sourceItems.Where(i => model.SelectedSourceItemIds.Contains(i.ItemId)).ToList();
			var selectedDestinationItems = destinationItems.Where(i => model.SelectedDestinationItemIds.Contains(i.ItemId)).ToList();

			var trade = new ItemTrade
			{
				SourceUser = new() { UserId = model.CurrentUserId.Value },
				DestinationUser = new() { UserId = model.SelectedUserId.Value },
				GameOfTrade = new() { GameId = model.SelectedGameId ?? 0 },
				TradeDescription = model.TradeDescription,
				TradeDate = DateTime.UtcNow,
				TradeStatus = "Pending",
				SourceUserItems = selectedSourceItems,
				DestinationUserItems = selectedDestinationItems,
				AcceptedBySourceUser = false,
				AcceptedByDestinationUser = false
			};

			await _tradeService.CreateTradeAsync(trade);

			model.SuccessMessage = "Trade offer created successfully!";
			return View("Index", await RebuildModel(model));
		}

		private async Task<TradeViewModel> RebuildModel(TradeViewModel model)
		{
			var allUsers = await _userService.GetAllUsersAsync();
			var games = await _gameService.GetAllGamesAsync();

			var sourceInventory = await _tradeService.GetUserInventoryAsync(model.CurrentUserId ?? 0);
			var destinationInventory = await _tradeService.GetUserInventoryAsync(model.SelectedUserId ?? 0);

			return new TradeViewModel
			{
				CurrentUserId = model.CurrentUserId,
				SelectedUserId = model.SelectedUserId,
				SelectedGameId = model.SelectedGameId,
				TradeDescription = model.TradeDescription,
				SelectedSourceItemIds = model.SelectedSourceItemIds ?? new(),
				SelectedDestinationItemIds = model.SelectedDestinationItemIds ?? new(),
				Users = allUsers.Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.UserName }).ToList(),
				AvailableUsers = allUsers.Where(u => u.UserId != model.CurrentUserId)
										 .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.UserName }).ToList(),
				Games = games.Select(g => new SelectListItem { Value = g.GameId.ToString(), Text = g.GameTitle }).ToList(),
				SourceUserItems = sourceInventory,
				DestinationUserItems = destinationInventory,
				SelectedSourceItems = sourceInventory.Where(i => model.SelectedSourceItemIds.Contains(i.ItemId)).ToList(),
				SelectedDestinationItems = destinationInventory.Where(i => model.SelectedDestinationItemIds.Contains(i.ItemId)).ToList(),
				ErrorMessage = model.ErrorMessage,
				SuccessMessage = model.SuccessMessage
			};
		}
	}
}
