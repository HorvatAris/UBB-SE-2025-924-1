using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.Web.ViewModels;
using System;
using System.Threading.Tasks;

namespace SteamHub.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index(int? selectedUserId, int? selectedGameId, string searchText)
        {
            var model = new InventoryViewModel();

            try
            {
                var user = inventoryService.GetAllUsers();
                if (user == null || user.UserId <= 0)
                {
                    model.StatusMessage = "No valid user found";
                    return View(model);
                }

                model.AvailableUsers.Add(user);

                model.SelectedUserId = (selectedUserId.HasValue && selectedUserId > 0)
                    ? selectedUserId
                    : user.UserId;

                if (model.SelectedUserId <= 0)
                {
                    model.StatusMessage = "Invalid user selected";
                    return View(model);
                }

                var filteredItems = await inventoryService.GetUserFilteredInventoryAsync(
                    model.SelectedUserId.Value,
                    selectedGameId.HasValue ? new Game { GameId = selectedGameId.Value } : null,
                    searchText);

                model.InventoryItems = filteredItems.ToList();

                var allItems = await inventoryService.GetUserInventoryAsync(model.SelectedUserId.Value);
                var availableGames = await inventoryService.GetAvailableGamesAsync(allItems);
                model.AvailableGames = availableGames.ToList();

                model.SelectedGameId = selectedGameId;
                model.SearchText = searchText;

                if (TempData["StatusMessage"] is string msg)
                {
                    model.StatusMessage = msg;
                }
            }
            catch (Exception ex)
            {
                model.StatusMessage = $"Error loading inventory: {ex.Message}";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sell(int itemId, string itemName)
        {
            try
            {
                // Confirm sale (in a real app, you might want JavaScript confirmation first)
                var item = new Item { ItemId = itemId };
                var success = await inventoryService.SellItemAsync(item);

                TempData["StatusMessage"] = success
                    ? $"{itemName} has been successfully listed for sale!"
                    : "Failed to sell the item. Please try again.";

                return RedirectToAction("Index", new
                {
                    selectedUserId = Request.Form["selectedUserId"],
                    selectedGameId = Request.Form["selectedGameId"],
                    searchText = Request.Form["searchText"]
                });
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = "Error selling item: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}