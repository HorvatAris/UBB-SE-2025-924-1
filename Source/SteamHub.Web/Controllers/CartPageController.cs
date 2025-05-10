using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Threading.Tasks;

namespace SteamHub.Web.Controllers
{
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
        [HttpGet]
        public async Task<IActionResult> PaypalPayment()
        {
            var amount = await cartService.GetTotalSumToBePaidAsync();
            var viewModel = new PaypalPaymentViewModel
            {
                AmountToPay = amount
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(string selectedPaymentMethod)
        {
            if (selectedPaymentMethod == "PayPal")
            {
                return RedirectToAction(nameof(PaypalPayment));
            }

            TempData["Error"] = "Selected payment method is not supported yet.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> PaypalPayment(PaypalPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var processor = new PaypalProcessor();
            var success = await processor.ProcessPaymentAsync(model.Email, model.Password, model.AmountToPay);

            if (success)
            {
                var games = await cartService.GetCartGamesAsync();
                await userGameService.PurchaseGamesAsync(games);
                await cartService.RemoveGamesFromCartAsync(games);

                // Instead of redirecting, show message
                model.IsSuccess = true;
                model.Message = "Payment successful!";
                model.PointsEarned = userGameService.LastEarnedPoints;

                return View(model);
            }

            model.IsSuccess = false;
            model.Message = "Payment failed. Please check your credentials.";
            return View(model);
        }

    }
}
