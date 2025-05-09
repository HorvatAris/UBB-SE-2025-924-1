// <copyright file="CreditCardPaymentViewModel.cs" company="PlaceholderCompany">
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
    using System.Windows.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SteamHub.ApiContract.Constants;
    using SteamHub.Pages;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.ApiContract.Models.Game;

    public class CreditCardPaymentViewModel : INotifyPropertyChanged
    {
        private const int ThresholdForNotEarningPoints = 0;
        private readonly ICartService cartService;
        private readonly IUserGameService userGameService;
        private readonly CreditCardProcessor creditCardProcessor;
        private string cardNumber;
        private string expirationDate;
        private string cvv;
        private string ownerName;
        private decimal totalAmount;
        private int lastEarnedPoints;

        public CreditCardPaymentViewModel(ICartService cartService, IUserGameService userGameService)
        {
            this.cartService = cartService;
            this.userGameService = userGameService;
            this.creditCardProcessor = new CreditCardProcessor();
            this.InitAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CardNumber
        {
            get => this.cardNumber;
            set
            {
                this.cardNumber = value;
                this.OnPropertyChanged();
            }
        }

        public string ExpirationDate
        {
            get => this.expirationDate;
            set
            {
                this.expirationDate = value;
                this.OnPropertyChanged();
            }
        }

        public string CVV
        {
            get => this.cvv;
            set
            {
                this.cvv = value;
                this.OnPropertyChanged();
            }
        }

        public string OwnerName
        {
            get => this.ownerName;
            set
            {
                this.ownerName = value;
                this.OnPropertyChanged();
            }
        }

        public decimal TotalAmount
        {
            get => this.totalAmount;
            private set
            {
                this.totalAmount = value;
                this.OnPropertyChanged();
            }
        }

        public int LastEarnedPoints
        {
            get => this.lastEarnedPoints;
            private set
            {
                this.lastEarnedPoints = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand ProcessPaymentCommand
        {
            get;
        }

        public async Task ProcessPaymentAsync(Frame frame)
        {
            bool paymentSuccess = await this.creditCardProcessor.ProcessPaymentAsync(this.cardNumber, this.expirationDate, this.cvv, this.ownerName);
            if (paymentSuccess)
            {
                List<Game> purchasedGames = await this.cartService.GetCartGamesAsync();
                await this.cartService.RemoveGamesFromCartAsync(purchasedGames);
                await this.userGameService.PurchaseGamesAsync(purchasedGames);
                this.LastEarnedPoints = this.userGameService.LastEarnedPoints;

                try
                {
                    Application.Current.Resources[ResourceKeys.RecentEarnedPoints] = this.LastEarnedPoints;
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine($"Error storing points: {exception.Message}");
                }

                if (this.LastEarnedPoints > ThresholdForNotEarningPoints)
                {
                    await this.ShowNotificationAsync(PaymentDialogStrings.PAYMENTSUCCESSTITLE, string.Format(PaymentDialogStrings.PAYMENTSUCCESSWITHPOINTSMESSAGE, this.LastEarnedPoints));
                }
                else
                {
                    await this.ShowNotificationAsync(PaymentDialogStrings.PAYMENTSUCCESSTITLE, PaymentDialogStrings.PAYMENTSUCCESSMESSAGE);
                }

                CartPage cartPage = new CartPage(this.cartService, this.userGameService);
                frame.Content = cartPage;
            }
            else
            {
                await this.ShowNotificationAsync(PaymentDialogStrings.PAYMENTFAILEDTITLE, PaymentDialogStrings.PAYMENTFAILEDMESSAGE);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task ShowNotificationAsync(string title, string message)
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

        private async void InitAsync()
        {
            this.TotalAmount = await this.cartService.GetTotalSumToBePaidAsync();
        }
    }
}