using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
				Users = allUsers.Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.UserName }).ToList(),
				AvailableUsers = allUsers.Where(user => user.UserId != currentUser.UserId)
										 .Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.UserName }).ToList(),
				Games = games.Select(game => new SelectListItem { Value = game.GameId.ToString(), Text = game.GameTitle }).ToList(),
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

			if (currentUser == null || model.SelectedUserId == null)
			{
				model.ErrorMessage = "Both users must be selected.";
				return View("Index", await RebuildModel(model));
			}

			if (!model.SelectedSourceItemIds.Any() && !model.SelectedDestinationItemIds.Any())
			{
				model.ErrorMessage = "Select at least one item to trade.";
				return View("Index", await RebuildModel(model));
			}

			var sourceItems = await _tradeService.GetUserInventoryAsync(currentUser.UserId);
			var destinationItems = await _tradeService.GetUserInventoryAsync(model.SelectedUserId.Value);

			var selectedSourceItems = sourceItems.Where(item => model.SelectedSourceItemIds.Contains(item.ItemId)).ToList();
			var selectedDestinationItems = destinationItems.Where(item => model.SelectedDestinationItemIds.Contains(item.ItemId)).ToList();

			var trade = new ItemTrade
			{
				SourceUser = new() { UserId = currentUser.UserId },
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

		public async Task<IActionResult> LoadSelectedUser(TradeViewModel model)
		{
			return View("Index", await RebuildModel(model));
		}

		public async Task<IActionResult> LoadSelectedGame(TradeViewModel model)
		{
			return View("Index", await RebuildModel(model));
		}

		private async Task<TradeViewModel> RebuildModel(TradeViewModel model)
		{
			var currentUser = _tradeService.GetCurrentUser();
			var allUsers = await _userService.GetAllUsersAsync();

			var games = await _gameService.GetAllGamesAsync();

			var sourceInventory = await _tradeService.GetUserInventoryAsync(currentUser.UserId);
			sourceInventory = sourceInventory.Where(item => item.Game.GameId == model.SelectedGameId).ToList();
			var destinationInventory = await _tradeService.GetUserInventoryAsync(model.SelectedUserId ?? 0);
			destinationInventory = destinationInventory.Where(item => item.Game.GameId == model.SelectedGameId).ToList();

			return new TradeViewModel
			{
				CurrentUserId = model.CurrentUserId,
				SelectedUserId = model.SelectedUserId,
				SelectedGameId = model.SelectedGameId,
				TradeDescription = model.TradeDescription,
				SelectedSourceItemIds = model.SelectedSourceItemIds ?? new(),
				SelectedDestinationItemIds = model.SelectedDestinationItemIds ?? new(),
				Users = allUsers.Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.UserName }).ToList(),
				AvailableUsers = allUsers.Where(user => user.UserId != currentUser.UserId)
										 .Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.UserName }).ToList(),
				Games = games.Select(game => new SelectListItem { Value = game.GameId.ToString(), Text = game.GameTitle }).ToList(),
				SourceUserItems = sourceInventory,
				DestinationUserItems = destinationInventory,
				SelectedSourceItems = sourceInventory.Where(item => model.SelectedSourceItemIds.Contains(item.ItemId)).ToList(),
				SelectedDestinationItems = destinationInventory.Where(item => model.SelectedDestinationItemIds.Contains(item.ItemId)).ToList(),
				ErrorMessage = model.ErrorMessage,
				SuccessMessage = model.SuccessMessage
			};
		}
	}
}
