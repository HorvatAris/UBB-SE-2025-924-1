using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketplaceController : ControllerBase
    {
        private readonly IMarketplaceService _marketplaceService;
        public MarketplaceController(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService ?? throw new ArgumentNullException(nameof(marketplaceService), "Marketplace service cannot be null");
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _marketplaceService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching users: {ex.Message}");
            }
        }
        [HttpGet("Listings")]
        public async Task<IActionResult> GetAllListings()
        {
            try
            {
                var listings = await _marketplaceService.GetAllListingsAsync();
                return Ok(listings);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching listings: {ex.Message}");
            }
        }

        [HttpGet("Listings/{userId}/Game/{gameId}")]
        public async Task<IActionResult> GetListingsByGame([FromRoute]int userId, [FromRoute] int gameId)
        {
            try
            {
                var listings = await _marketplaceService.GetListingsByGameAsync(gameId, userId);
                return Ok(listings);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching listings for game ID {gameId}: {ex.Message}");
            }
        }

        [HttpPut("UpdateListing/{gameId}/{itemId}")]
        public async Task<IActionResult> UpdateListing([FromRoute] int gameId, [FromRoute] int itemId)
        {
            try
            {
                await _marketplaceService.UpdateListingAsync(gameId, itemId);
                return Ok("Listing updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the listing: {ex.Message}");
            }
        }

        [HttpPost("BuyItem/{userId}")]
        public async Task<IActionResult> BuyItem([FromRoute] int userId, [FromBody] Item item)
        {
            if (item == null)
            {
                return BadRequest("Item cannot be null.");
            }
            try
            {
                var result = await _marketplaceService.BuyItemAsync(item, userId);
                return Ok("Item purchased successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"An error occurred while purchasing the item: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("SwitchListing/{gameId}/{itemId}")]
        public async Task<IActionResult> SwitchListing([FromRoute] int gameId, [FromRoute] int itemId)
        {
            try
            {
                await this._marketplaceService.SwitchListingStatusAsync(gameId, itemId);
                return Ok("Listing status switched successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while switching the listing status: {ex.Message}");
            }
        }

    }
}
