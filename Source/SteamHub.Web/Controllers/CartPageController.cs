using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Threading.Tasks;

namespace SteamHub.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class CartPageController : Controller
    {
        private readonly ICartService cartService;
        private readonly IUserGameService userGameService;

        public CartPageController(ICartService cartService, IUserGameService userGameService)
        {
            this.cartService = cartService;
            this.userGameService = userGameService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var games = await this.cartService.GetCartGamesAsync();
                var model = new CartPageViewModel
                {
                    CartGames = games,
                    TotalPrice = await this.cartService.GetTotalSumToBePaidAsync(),

                };
                return View(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching cart games: {ex.Message}");
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int gameId)
        {
            var game = (await this.cartService.GetCartGamesAsync()).FirstOrDefault(g => g.GameId == gameId);

            if (game != null)
            {
                await cartService.RemoveGameFromCartAsync(game);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
