using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class GamePageController : Controller
    {
        private readonly IGameService gameService;
        private readonly ICartService cartService;
        private readonly IUserGameService userGameService;

        public GamePageController(IGameService gameService, ICartService cartService, IUserGameService userGameService)
        {
            this.gameService = gameService;
            this.cartService = cartService;
            this.userGameService = userGameService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var game = await gameService.GetGameByIdAsync(id);
            if (game == null) return NotFound();

            var isOwned = await userGameService.IsGamePurchasedAsync(game);
            var tags = await gameService.GetAllGameTagsAsync(game);
            var similarGames = await gameService.GetSimilarGamesAsync(game.GameId);

            var vm = new GamePageViewModel
            {
                Game = game,
                IsOwned = isOwned,
                GameTags = tags.Select(tag => tag.Tag_name).ToList(),
                MediaLinks = new List<string> { game.TrailerPath, game.GameplayPath }.Where(p => !string.IsNullOrEmpty(p)).ToList(),
                SimilarGames = similarGames.Take(3).ToList()
            };

            return View(vm);
        }

        //[HttpPost]
        //public async Task<IActionResult> AddToCart(int id)
        //{
        //    var game = await gameService.GetGameByIdAsync(id);
        //    if (game == null) return NotFound();

        //    await cartService.AddGameToCartAsync(game);
        //    return RedirectToAction("Details", new { id });
        //}
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            try
            {
                var game = await gameService.GetGameByIdAsync(id);
                if (game == null) return Json(new { success = false, message = "Game not found." });

                await cartService.AddGameToCartAsync(game);
                return Json(new { success = true, message = "Game added to cart successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> AddToWishlist(int id)
        //{
        //    var game = await gameService.GetGameByIdAsync(id);
        //    if (game == null) return NotFound();

        //    await userGameService.AddGameToWishlistAsync(game);
        //    return RedirectToAction("Details", new { id });
        //}
        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int id)
        {
            try
            {
                var game = await gameService.GetGameByIdAsync(id);
                if (game == null)
                    return Json(new { success = false, message = "Game not found." });

                await userGameService.AddGameToWishlistAsync(game);
                return Json(new { success = true, message = "Game added to wishlist successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


    }
}
