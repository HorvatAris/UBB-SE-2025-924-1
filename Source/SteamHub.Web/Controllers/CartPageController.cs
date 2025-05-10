using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Threading.Tasks;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class CartPageController : Controller
    {
        private readonly ICartService cartService;
        private readonly IUserGameService userGameService;
        private readonly IUserDetails user;

        public CartPageController(ICartService cartService, IUserGameService userGameService)
        {
            this.cartService = cartService;
            this.userGameService = userGameService;
            this.user = this.cartService.GetUser();
        }

        public async Task<IActionResult> Index()
        {
            var games = await cartService.GetCartGamesAsync();
            var model = new CartPageViewModel
            {
                CartGames = games,
                TotalPrice = await cartService.GetTotalSumToBePaidAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int gameId)
        {
            var game = (await cartService.GetCartGamesAsync()).FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
                await cartService.RemoveGameFromCartAsync(game);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string selectedPaymentMethod)
        {
            if (selectedPaymentMethod == "PayPal")
                return RedirectToAction(nameof(PaypalPayment));

            if (selectedPaymentMethod == "Credit Card")
                return RedirectToAction(nameof(CreditCardPayment));

            if (selectedPaymentMethod == "Steam Wallet")
                return await SteamWalletPayment();

            TempData["Error"] = "Selected payment method is not supported.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> PaypalPayment()
        {
            var viewModel = new PaypalPaymentViewModel
            {
                AmountToPay = await cartService.GetTotalSumToBePaidAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PaypalPayment(PaypalPaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var processor = new PaypalProcessor();
            bool success = await processor.ProcessPaymentAsync(model.Email, model.Password, model.AmountToPay);

            if (success)
            {
                var games = await cartService.GetCartGamesAsync();
                await userGameService.PurchaseGamesAsync(games);
                await cartService.RemoveGamesFromCartAsync(games);

                model.IsSuccess = true;
                model.PointsEarned = userGameService.LastEarnedPoints;
                model.Message = $"Payment successful! You earned {model.PointsEarned} points.";

                return View(model);
            }

            model.IsSuccess = false;
            model.Message = "Payment failed. Please check your credentials.";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreditCardPayment()
        {
            var viewModel = new CreditCardPaymentViewModel
            {
                TotalAmount = await cartService.GetTotalSumToBePaidAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreditCardPayment(CreditCardPaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var processor = new CreditCardProcessor();
            bool success = await processor.ProcessPaymentAsync(model.CardNumber, model.ExpirationDate, model.CVV, model.OwnerName);

            if (success)
            {
                var games = await cartService.GetCartGamesAsync();
                await userGameService.PurchaseGamesAsync(games);
                await cartService.RemoveGamesFromCartAsync(games);

                model.IsSuccess = true;
                model.PointsEarned = userGameService.LastEarnedPoints;
                model.Message = $"Payment successful! You earned {model.PointsEarned} points.";

                return View(model); // Stay on the same page to show success
            }

            model.IsSuccess = false;
            model.Message = "Payment failed. Please try again.";
            return View(model);
        }

        private async Task<IActionResult> SteamWalletPayment()
        {
            var total = await cartService.GetTotalSumToBePaidAsync();
            if (user.WalletBalance < (float)total)
            {
                TempData["Error"] = "Insufficient Steam Wallet funds.";
                return RedirectToAction(nameof(Index));
            }

            var games = await cartService.GetCartGamesAsync();
            await userGameService.PurchaseGamesAsync(games);
            await cartService.RemoveGamesFromCartAsync(games);

            TempData["PointsEarned"] = userGameService.LastEarnedPoints;
            return RedirectToAction("Index", "HomePage");
        }

    }
}
