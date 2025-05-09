using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Context.Repositories;
using SteamHub.ApiContract.Models.UserPointShopItemInventory;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPointShopItemInventoryController : ControllerBase
    {
        private readonly IUserPointShopItemInventoryRepository _userPointShopItemInventoryRepository;
        public UserPointShopItemInventoryController(IUserPointShopItemInventoryRepository userPointShopItemInventoryRepository)
        {
            _userPointShopItemInventoryRepository = userPointShopItemInventoryRepository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserInventory(int userId)
        {
            var result = await _userPointShopItemInventoryRepository.GetUserInventoryAsync(userId);
            return Ok(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseItem([FromBody] PurchasePointShopItemRequest request)
        {
            try
            {
                await _userPointShopItemInventoryRepository.PurchaseItemAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateItemStatus([FromBody] UpdateUserPointShopItemInventoryRequest request)
        {
            try
            {
                await _userPointShopItemInventoryRepository.UpdateItemStatusAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }
    }
}
