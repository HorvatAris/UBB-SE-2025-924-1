using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class PointShopPageController : Controller
    {
        private readonly IPointShopService _pointShopService;

        public PointShopPageController(IPointShopService pointShopService)
        {
            _pointShopService = pointShopService;
        }

        public async Task<IActionResult> Index()
        {
            var user = _pointShopService.GetCurrentUser();
            var shopItems = await _pointShopService.GetAvailableItemsAsync(user);
            var userItems = await _pointShopService.GetUserItemsAsync();

            var viewModel = new PointShopViewModel
            {
                User = user,
                ShopItems = shopItems,
                UserItems = userItems,
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
                await _pointShopService.PurchaseItemAsync(selectedItem);
                return Json(new { success = true, message = $"Successfully purchased {selectedItem.Name}." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to purchase item: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ActivateItem(int itemId)
        {
            var userItems = await _pointShopService.GetUserItemsAsync();
            var selectedItem = userItems.FirstOrDefault(item => item.ItemIdentifier == itemId);

            if (selectedItem == null)
            {
                return Json(new { success = false, message = "Item not found in your inventory." });
            }

            try
            {
                await _pointShopService.ActivateItemAsync(selectedItem);
                return Json(new { success = true, message = $"{selectedItem.Name} has been activated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to activate item: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateItem(int itemId)
        {
            var userItems = await _pointShopService.GetUserItemsAsync();
            var selectedItem = userItems.FirstOrDefault(item => item.ItemIdentifier == itemId);

            if (selectedItem == null)
            {
                return Json(new { success = false, message = "Item not found in your inventory." });
            }

            try
            {
                await _pointShopService.DeactivateItemAsync(selectedItem);
                return Json(new { success = true, message = $"{selectedItem.Name} has been deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to deactivate item: {ex.Message}" });
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
            var userItems = await _pointShopService.GetUserItemsAsync();

            return Json(userItems.Select(item => new
            {
                item.ItemIdentifier,
                item.Name,
                item.ItemType,
                item.ImagePath,
                item.IsActive
            }));
        }


    }
}
