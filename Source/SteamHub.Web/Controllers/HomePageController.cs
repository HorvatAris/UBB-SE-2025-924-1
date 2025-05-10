using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Authorization;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class HomePageController : Controller
    {
        private readonly IGameService gameService;
        private readonly IUserGameService userGameService;
        private readonly ICartService cartService;

        public HomePageController(IGameService gameService, IUserGameService userGameService, ICartService cartService)
        {
            this.gameService = gameService;
            this.userGameService = userGameService;
            this.cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var filteredGames = await gameService.GetAllApprovedGamesAsync();
            var trendingGames = await gameService.GetTrendingGamesAsync();
            var recommendedGames = await userGameService.GetRecommendedGamesAsync();
            var discountedGames = await gameService.GetDiscountedGamesAsync();
            var tags = await gameService.GetAllTagsAsync();

            var model = new HomePageViewModel
            {
                SearchFilterText = "All Games",
                TrendingGames = trendingGames,
                RecommendedGames = recommendedGames,
                DiscountedGames = discountedGames,
                Tags = tags,
                FilteredGames = filteredGames,
            };

            return View(model);
        }

        // GET: Home/SearchGames
        [HttpGet]
        public async Task<IActionResult> SearchGames(string query)
        {
            var games = await gameService.SearchGamesAsync(query);
            return Json(games);
        }

        // GET: Home/ApplyFilters
        [HttpGet]
        public async Task<IActionResult> ApplyFilters(int rating, int minPrice, int maxPrice, string[] tags)
        {
            var filteredGames = await gameService.FilterGamesAsync(rating, minPrice, maxPrice, tags);
            return Json(filteredGames);
        }

        [HttpGet]
        public async Task<IActionResult> GetGameCard(int gameId)
        {
            var game = await gameService.GetGameByIdAsync(gameId);

            if (game == null)
            {
                return NotFound();
            }

            // Return the rendered partial view for a single game card
            return PartialView("_GameCard", game);
        }
    }
}
