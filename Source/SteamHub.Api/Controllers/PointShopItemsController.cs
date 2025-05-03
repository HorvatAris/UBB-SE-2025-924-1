using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.PointShopItem;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointShopItemsController : ControllerBase
    {
        private readonly IPointShopItemRepository _pointShopItemRepository;

        public PointShopItemsController(IPointShopItemRepository pointShopItemRepository)
        {
            _pointShopItemRepository = pointShopItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _pointShopItemRepository.GetPointShopItemsAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _pointShopItemRepository.GetPointShopItemByIdAsync(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePointShopItemRequest request)
        {
            try
            {
                await _pointShopItemRepository.UpdatePointShopItemAsync(id, request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePointShopItemAsync([FromBody] CreatePointShopItemRequest request)
        {
            try
            {
                var existingPointShopItem = await _pointShopItemRepository.CreatePointShopItemAsync(request);
                return Ok(existingPointShopItem);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePointShopItemAsync([FromRoute] int id)
        {
            try
            {
                await _pointShopItemRepository.DeletePointShopItemAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }
    }
}