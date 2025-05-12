using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Item;
using SteamHub.Web.ViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryService inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? selectedUserId, int? selectedGameId, string searchText)
        {
            var user = inventoryService.GetAllUsers();
            var users = new List<User> { user };
            var currentUserId = selectedUserId ?? users.FirstOrDefault()?.UserId ?? 0;

            var allItems = await inventoryService.GetUserInventoryAsync(currentUserId);
            var filteredItems = await inventoryService.GetUserFilteredInventoryAsync(
                currentUserId,
                selectedGameId.HasValue ? new Game { GameId = selectedGameId.Value } : null,
                searchText
            );

            var availableGames = await inventoryService.GetAvailableGamesAsync(allItems);

            var model = new InventoryViewModel
            {
                SelectedUserId = currentUserId,
                SelectedGameId = selectedGameId,
                SearchText = searchText,
                InventoryItems = filteredItems,
                AvailableGames = availableGames,
                AvailableUsers = users
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sell(int itemId, int selectedUserId, int? selectedGameId, string searchText)
        {
            var item = (await inventoryService.GetUserInventoryAsync(selectedUserId))
                .FirstOrDefault(i => i.ItemId == itemId);

            if (item != null && !item.IsListed)
                await inventoryService.SellItemAsync(item);

            TempData["StatusMessage"] = item != null
                ? $"Item '{item.ItemName}' was successfully listed for sale."
                : "Item could not be found or is already listed.";

            return RedirectToAction(nameof(Index), new { selectedUserId, selectedGameId, searchText });
        }
    }
}