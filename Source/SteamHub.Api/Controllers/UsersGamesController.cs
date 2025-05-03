using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Models.UserPointShopItemInventory;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersGamesController : ControllerBase
    {
        private readonly IUsersGamesRepository _usersGamesRepository;
        public UsersGamesController(IUsersGamesRepository usersGamesRepository)
        {
            _usersGamesRepository = usersGamesRepository;
        }

        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetUserGames(int userId)
        {
            var result = await _usersGamesRepository.GetUserGamesAsync(userId);
            return Ok(result);
        }

        [HttpGet("cart/{userId}")]
        public async Task<IActionResult> GetUserCart(int userId)
        {
            var result = await _usersGamesRepository.GetUserCartAsync(userId);
            return Ok(result);
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] UserGameRequest request)
        {
            try
            {
                await _usersGamesRepository.AddToCartAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpPatch("remove-from-cart")]
        public async Task<IActionResult> RemoveFromCart([FromBody] UserGameRequest request)
        {
            try
            {
                await _usersGamesRepository.RemoveFromCartAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpGet("wishlist/{userId}")]
        public async Task<IActionResult> GetUserWishlist(int userId)
        {
            var result = await _usersGamesRepository.GetUserWishlistAsync(userId);
            return Ok(result);
        }

        [HttpPost("add-to-wishlist")]
        public async Task<IActionResult> AddToWishlist([FromBody] UserGameRequest request)
        {
            try
            {
                await _usersGamesRepository.AddToWishlistAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpPatch("remove-from-wishlist")]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] UserGameRequest request)
        {
            try
            {
                await _usersGamesRepository.RemoveFromWishlistAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseGame([FromBody] UserGameRequest request)
        {
            try
            {
                await _usersGamesRepository.PurchaseGameAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpGet("purchased/{userId}")]
        public async Task<IActionResult> GetUserPurchasedGames(int userId)
        {
            var result = await _usersGamesRepository.GetUserPurchasedGamesAsync(userId);
            return Ok(result);
        }
    }
}