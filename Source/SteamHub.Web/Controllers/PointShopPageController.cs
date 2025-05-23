using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.UserPointShopItemInventory;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Collections.ObjectModel;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class PointShopPageController : Controller
    {
        private readonly IPointShopService _pointShopService;
        private static ObservableCollection<PointShopTransaction> transactionHistory = new ObservableCollection<PointShopTransaction>();

        public PointShopPageController(IPointShopService pointShopService)
        {
            _pointShopService = pointShopService;
        }

        public async Task<IActionResult> Index()
        {
            var user = _pointShopService.GetCurrentUser();
            var shopItems = await _pointShopService.GetAvailableItemsAsync(user);
            var userItems = await _pointShopService.GetUserItemsAsync(user.UserId);


            var viewModel = new PointShopViewModel
            {
                User = user,
                ShopItems = shopItems,
                UserItems = userItems,
                TransactionHistory = transactionHistory.ToList(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseItem([FromQuery]int itemId)
        {
            Console.WriteLine($"Received itemId: {itemId}");
            var user = _pointShopService.GetCurrentUser();
            var shopItems = await _pointShopService.GetAvailableItemsAsync(user);
            var selectedItem = shopItems.FirstOrDefault(item => item.ItemIdentifier == itemId);

            if (selectedItem == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }

            try
            {
                var request = new PurchasePointShopItemRequest
                {
                    PointShopItemId = selectedItem.ItemIdentifier,
                    UserId = user.UserId
                };
                await _pointShopService.PurchaseItemAsync(request);
                var newTransaction = new PointShopTransaction(
                    transactionHistory.Count + 1,
                    selectedItem.Name,
                    selectedItem.PointPrice,
                    selectedItem.ItemType,
                    user.UserId
                );
                transactionHistory.Add(newTransaction);
                Console.WriteLine($"[DEBUG] Transaction added: {newTransaction.ItemName}, PointsSpent: {newTransaction.PointsSpent}");
                return Json(new { success = true, message = $"Successfully purchased {selectedItem.Name}." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to purchase item: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActivation(int id)
        {
            var user = _pointShopService.GetCurrentUser();
            var userItems = await _pointShopService.GetUserItemsAsync(user.UserId);
            var selectedItem = userItems.FirstOrDefault(item => item.ItemIdentifier == id);

            if (selectedItem == null)
            {
                return Json(new { success = false, message = "Item not found in your inventory." });
            }

            try
            {
                var request = new UpdateUserPointShopItemInventoryRequest
                {
                    PointShopItemId = selectedItem.ItemIdentifier,
                    UserId = user.UserId,
                    IsActive = !selectedItem.IsActive  
                };
                if (selectedItem.IsActive)
                {
                    await _pointShopService.DeactivateItemAsync(request);
                    return Json(new { success = true, message = $"{selectedItem.Name} has been deactivated." });
                }
                else
                {
                    await _pointShopService.ActivateItemAsync(request);
                    return Json(new { success = true, message = $"{selectedItem.Name} has been activated." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to toggle activation: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ApplyFilters(string search, string type, int maxPrice)
        {
            var user = _pointShopService.GetCurrentUser();
            var allItems = await _pointShopService.GetAvailableItemsAsync(user);

            var filteredItems = allItems
                .Where(item =>
                    (string.IsNullOrEmpty(search) || item.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
                    (type == "All" || item.ItemType.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
                    item.PointPrice <= maxPrice)
                .ToList();

            return Json(filteredItems.Select(item => new
            {
                item.ItemIdentifier,
                item.Name,
                item.ItemType,
                item.PointPrice,
                item.ImagePath
            }));
        }

        [HttpGet]
        public async Task<IActionResult> GetMaxPrice()
        {
            var user = _pointShopService.GetCurrentUser();
            var allItems = await _pointShopService.GetAvailableItemsAsync(user);

            var maxPrice = allItems.Max(item => item.PointPrice);

            return Json(new { maxPrice });
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {
            var user = _pointShopService.GetCurrentUser();
            var userItems = await _pointShopService.GetUserItemsAsync(user.UserId);

            return Json(userItems.Select(item => new
            {
                item.ItemIdentifier,
                item.Name,
                item.ItemType,
                item.ImagePath,
                item.IsActive
            }));
        }

        [HttpGet]
        public IActionResult GetTransactionHistory()
        {
            var user = _pointShopService.GetCurrentUser();
            var userTransactions = transactionHistory.Where(t => t.UserId == user.UserId).ToList();
            return Json(userTransactions);
        }
    }
}
