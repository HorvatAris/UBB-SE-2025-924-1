using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context.Repositories;
using SteamHub.ApiContract.Models.UserInventory;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInventoryController : ControllerBase
    {
        private readonly IUserInventoryRepository _userInventoryRepository;

        public UserInventoryController(IUserInventoryRepository userInventoryRepository)
        {
            _userInventoryRepository = userInventoryRepository;
        }

        // GET: api/UserInventory/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserInventory(int userId)
        {
            var response = await _userInventoryRepository.GetUserInventoryAsync(userId);
            if (response == null || response.Items.Count == 0)
            {
                return NotFound($"No inventory found for user with ID {userId}.");
            }

            return Ok(response);
        }

        // GET: api/UserInventory/{userId}/item/{itemId}
        [HttpGet("{userId}/item/{itemId}")]
        public async Task<IActionResult> GetItemFromUserInventory(int userId, int itemId)
        {
            var response = await _userInventoryRepository.GetItemFromUserInventoryAsync(userId, itemId);
            if (response == null)
            {
                return NotFound($"Item with ID {itemId} not found in inventory for user with ID {userId}.");
            }

            return Ok(response);
        }

        // POST: api/UserInventory
        [HttpPost]
        public async Task<IActionResult> AddItemToUserInventory([FromBody] ItemFromInventoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userInventoryRepository.AddItemToUserInventoryAsync(request);
                return CreatedAtAction(nameof(GetItemFromUserInventory), new { userId = request.UserId, itemId = request.ItemId }, request);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/UserInventory
        [HttpDelete]
        public async Task<IActionResult> RemoveItemFromUserInventory([FromBody] ItemFromInventoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userInventoryRepository.RemoveItemFromUserInventoryAsync(request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

