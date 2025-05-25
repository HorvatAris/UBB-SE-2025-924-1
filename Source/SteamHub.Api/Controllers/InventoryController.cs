using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        [HttpPost("AddItemToInventory/{userId}")]
        public async Task<IActionResult> AddItemToInventory([FromBody] AddItemToInventoryRequest request, [FromRoute] int userId)
        {
            try
            {
                await inventoryService.AddItemToInventoryAsync(request.Game, request.Item,userId);
                return Ok(new { message = "Item added to inventory." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("All/{userId}")]
        public async Task<IActionResult> GetAllItemsFromInventory([FromRoute] int userId)
        {
            try
            {
                var items = await this.inventoryService.GetAllItemsFromInventoryAsync(userId);
                return Ok(items);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("AvailableGames/{userId}")]
        public async Task<IActionResult> GetAvailableGames([FromRoute] int userId, [FromBody] List<Item> items)
        {
            try
            {
                var games = await this.inventoryService.GetAvailableGamesAsync(items, userId);
                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("ItemsFromGame/{userId}")]
        public async Task<IActionResult> GetItemsFromInventory([FromRoute] int userId, [FromBody] Game game)
        {
            try
            {
                var items = await this.inventoryService.GetItemsFromInventoryAsync(game, userId);
                return Ok(items);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("UserInventory/{userId}")]
        public async Task<IActionResult> GetUserInventory([FromRoute] int userId)
        {
            try
            {
                var items = await inventoryService.GetUserInventoryAsync(userId);
                return Ok(items);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("SellItem/{userId}")]
        public async Task<IActionResult> SellItem([FromRoute] int userId, [FromBody] Item item)
        {
            if (item == null)
            {
                return BadRequest("Item must be provided.");
            }

            try
            {
                bool success = await this.inventoryService.SellItemAsync(item, userId);
                if (success)
                    return Ok(new { message = "Item listed for sale successfully." });
                else
                    return StatusCode(500, new { error = "Failed to sell item." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
