// <copyright file="PaypalPaymentViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SteamHub.ApiContract.Constants;
    using SteamHub.Pages;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UsersGames;

    public class PaypalPaymentViewModel : INotifyPropertyChanged
    {
        private const int NoPointsEarnedAmount = 0;
        private ICartService cartService;
        private IUserGameService userGameService;
        private IUserDetails user;
        private List<Game> purchasedGames;
        private PaypalProcessor paypalProcessor;
        private decimal amountToPay;
        private string email;
        private string password;

        // Updated constructor to be async and return Task
        public PaypalPaymentViewModel(ICartService cartService, IUserGameService userGameService)
        {
            this.cartService = cartService;
            this.userGameService = userGameService;
            this.user = this.cartService.GetUser();
            this.paypalProcessor = new PaypalProcessor();
            this.InitAmountToPayAsync();
            this.InitAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Email
            {
                get => this.email;
                set
                {
                    this.email = value;
                    this.OnPropertyChanged();
                }
            }

        public string Password
        {
            get => this.password;
            set
            {
                this.password = value;
                this.OnPropertyChanged();
            }
        }

        // Added an async factory method to initialize the ViewModel
        public static async Task<PaypalPaymentViewModel> CreateAsync(ICartService cartService, IUserGameService userGameService)
        {
            var viewModel = new PaypalPaymentViewModel(cartService, userGameService);
            viewModel.InitAsync();
            return viewModel;
        }

        public async void InitAsync()
        {
            this.purchasedGames = await this.cartService.GetCartGamesAsync(this.user.UserId);
            System.Diagnostics.Debug.WriteLine($"Purchased games count: {this.purchasedGames.Count}");
        }

        public async Task ValidatePayment(Frame frame)
        {
            bool paymentSuccess = await this.paypalProcessor.ProcessPaymentAsync(this.Email, this.Password, this.amountToPay);
            if (paymentSuccess)
            {
                await this.cartService.RemoveGamesFromCartAsync(this.purchasedGames);
                var request = new PurchaseGamesRequest
                {
                    UserId = this.user.UserId,
                    Games = this.purchasedGames.ToList(),
                    IsWalletPayment = false,
                };
                await this.userGameService.PurchaseGamesAsync(request);

                // Get points earned from the purchase
                int pointsEarned = this.userGameService.LastEarnedPoints;
                System.Diagnostics.Debug.WriteLine($"Points earned: {pointsEarned}");   

                // Store points in App resources for PointsShopPage to access
                try
                {
                    Application.Current.Resources[ResourceKeys.RecentEarnedPoints] = pointsEarned;
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine($"Error storing points: {exception.Message}");
                }

                // Show points earned notification if points were earned
                if (pointsEarned > NoPointsEarnedAmount)
                {
                    await this.ShowNotification(PaymentDialogStrings.PAYMENTSUCCESSMESSAGE, string.Format(PaymentDialogStrings.PAYMENTSUCCESSWITHPOINTSMESSAGE, pointsEarned));
                }
                else
                {
                    await this.ShowNotification(PaymentDialogStrings.PAYMENTSUCCESSTITLE, PaymentDialogStrings.PAYMENTSUCCESSMESSAGE);
                }

                frame.Content = new CartPage(this.cartService, this.userGameService);
            }
            else
            {
                await this.ShowNotification(PaymentDialogStrings.PAYMENTFAILEDTITLE, PaymentDialogStrings.PAYMENTFAILEDMESSAGE);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void InitAmountToPayAsync()
        {
            this.amountToPay = await this.cartService.GetTotalSumToBePaidAsync();
        }

        private async Task ShowNotification(string title, string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = PaymentDialogStrings.OKBUTTONTEXT,
                XamlRoot = App.MainWindow.Content.XamlRoot,
            };
            await dialog.ShowAsync();
        }
    }
}
