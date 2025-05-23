using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using System.Collections.ObjectModel;
using SteamHub.ApiContract.Models.UserPointShopItemInventory;

namespace SteamHub.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PointShopController : ControllerBase
    {
        private readonly IPointShopService pointShopService;

        public PointShopController(IPointShopService pointShopService)
        {
            this.pointShopService = pointShopService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await pointShopService.GetAllItemsAsync();

            return Ok(result);
        }

        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetUserItems([FromRoute] int userId)
        {
            var result = await pointShopService.GetUserItemsAsync(userId);

            return Ok(result);
        }

        [HttpPost("Purchase")]
        public async Task<IActionResult> PurchaseItem([FromBody] PurchasePointShopItemRequest request)
        {
            try
            {
                await pointShopService.PurchaseItemAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpPut("Activate")]
        public async Task<IActionResult> ActivateItem([FromBody] UpdateUserPointShopItemInventoryRequest request)
        {
            try
            {
                await pointShopService.ActivateItemAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpPut("Deactivate")]
        public async Task<IActionResult> DeactivateItem([FromBody] UpdateUserPointShopItemInventoryRequest request)
        {
            try
            {
                await pointShopService.DeactivateItemAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }
    }
}
