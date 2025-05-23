using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserCart(int userId)
        {
            var result = await cartService.GetCartGamesAsync(userId);
            return Ok(result);
        }

        [HttpGet("Purchased/{userId}")]
        public async Task<IActionResult> GetUserPurchasedGames(int userId)
        {
            var result = await cartService.GetAllPurchasedGamesAsync(userId);
            return Ok(result);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] UserGameRequest request)
        {
            try
            {
                await cartService.AddGameToCartAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

        [HttpPatch("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart([FromBody] UserGameRequest request)
        {
            try
            {
                await cartService.RemoveGameFromCartAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

    }
}
