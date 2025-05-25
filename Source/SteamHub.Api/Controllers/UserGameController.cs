using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserGameController : ControllerBase
    {
        private readonly IUserGameService _userGameService;
        public UserGameController(IUserGameService userGameService)
        {
            _userGameService = userGameService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserGames(int userId)
        {
            var result = await _userGameService.GetAllGamesAsync(userId);
            return Ok(result);
        }
        [HttpGet("Wishlist/{userId}")]
        public async Task<IActionResult> GetUserWishlist(int userId)
        {
            var result = await _userGameService.GetWishListGamesAsync(userId);
            return Ok(result);
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddToWishlist([FromBody] UserGameRequest request)
        {
            try
            {
                await _userGameService.AddGameToWishlistAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

        [HttpPatch("RemoveFromWishlist")]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] UserGameRequest request)
        {
            try
            {
                await _userGameService.RemoveGameFromWishlistAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
            return NoContent();
        }

        [HttpGet("Purchased/{userId}")]
        public async Task<IActionResult> GetUserPurchasedGames(int userId)
        {
            var result = await _userGameService.GetPurchasedGamesAsync(userId);
            return Ok(result);
        }

        [HttpGet("RecommendedGames/{userId}")]
        public async Task<IActionResult> GetRecommendedGames([FromRoute]int userId)
        {
            var result = await _userGameService.GetRecommendedGamesAsync(userId);
            return Ok(result);
        }

        [HttpPost("Purchase")]
        public async Task<IActionResult> PurchaseGames([FromBody] PurchaseGamesRequest request)
        {
            try
            {
                int earnedPoints = await _userGameService.PurchaseGamesAsync(request);

                var response = new PurchaseResponse
                {
                    PointsEarned = earnedPoints
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("Tags/{userId}")]
        public async Task<IActionResult> GetFavoriteUserTags([FromRoute] int userId)
        {
            var result = await _userGameService.GetFavoriteUserTagsAsync(userId);
            return Ok(result);
        }
    }
}
