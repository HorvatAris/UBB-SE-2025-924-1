// <copyright file="CartViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub;
using SteamHub.ApiContract.Constants;
using SteamHub.Pages;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;
using SteamHub.ApiContract.Models.Game;

public class CartViewModel : INotifyPropertyChanged
{
    private const int ThresholdForNotEarningPoints = 0;
    private const int InitialValueForLastEarnedPoints = 0;
    private ICartService cartService;

    private IUserGameService userGameService;
    private ObservableCollection<Game> cartGames;

    private decimal totalPrice;

    private string selectedPaymentMethod;

    public CartViewModel(ICartService cartService, IUserGameService userGameService)
    {
        this.cartService = cartService;
        this.userGameService = userGameService;
        this.CartGames = new ObservableCollection<Game>();
        this.LastEarnedPoints = InitialValueForLastEarnedPoints;
        this.LoadGamesAsync();

        // Initialize commands
        this.RemoveGameCommand = new RelayCommand<Game>(this.RemoveGameFromCartAsync);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Game> CartGames
    {
        get => this.cartGames;
        set
        {
            this.cartGames = value;
            this.OnPropertyChanged();
            this.UpdateTotalPrice();
        }
    }

    public decimal TotalPrice
    {
        get => this.totalPrice;
        private set
        {
            if (this.totalPrice != value)
            {
                this.totalPrice = value;
                this.OnPropertyChanged();
            }
        }
    }

    public string SelectedPaymentMethod
    {
        get => this.selectedPaymentMethod;
        set
        {
            this.selectedPaymentMethod = value;
            this.OnPropertyChanged();
        }
    }

    // Property to track points earned in the last purchase
    public int LastEarnedPoints { get; private set; }

    public ICommand RemoveGameCommand { get; }

    public ICommand CheckoutCommand { get; }

    public float ShowUserFunds()
    {
        return this.cartService.GetUserFunds();
    }

    public async void RemoveGameFromCartAsync(Game game)
    {
        await this.cartService.RemoveGameFromCartAsync(game);
        this.CartGames.Remove(game);
        this.UpdateTotalPrice();
        this.OnPropertyChanged(nameof(this.CartGames));
    }

    public async Task PurchaseGamesAsync()
    {
        bool isWalletPayment = false;
        if (this.SelectedPaymentMethod == PaymentMethods.SteamWalletPaymentWallet)
            isWalletPayment = true;

        await this.userGameService.PurchaseGamesAsync(this.CartGames.ToList(), isWalletPayment);

        // Get the points earned from the user game service
        this.LastEarnedPoints = this.userGameService.LastEarnedPoints;

        await this.cartService.RemoveGamesFromCartAsync(this.CartGames.ToList());
        this.CartGames.Clear();
        this.UpdateTotalPrice();
    }

    public async void ChangeToPaymentPageAsync(Frame frame)
    {
        if (this.SelectedPaymentMethod == PaymentMethods.PayPalPaymentMethods)
        {
            PaypalPaymentPage paypalPaymentPage = new PaypalPaymentPage(this.cartService, this.userGameService);
            frame.Content = paypalPaymentPage;
        }
        else if (this.SelectedPaymentMethod == PaymentMethods.CreditCardPaymentMethod)
        {
            CreditCardPaymentPage creditCardPaymentPage = new CreditCardPaymentPage(this.cartService, this.userGameService);
            frame.Content = creditCardPaymentPage;
        }
        else if (this.SelectedPaymentMethod == PaymentMethods.SteamWalletPaymentWallet)
        {
            float totalPrice = this.cartService.GetTheTotalSumOfItemsInCart(this.CartGames.ToList());

            // float totalPrice = this.userGameService.ComputeSumOfGamesInCart(this.CartGames.ToList());
            float userFunds = this.ShowUserFunds();
            if (userFunds < totalPrice)
            {
                await this.ShowDialog(InsufficientFundsErrors.INSUFFICIENTFUNDSERRORTITLE, InsufficientFundsErrors.INSUFFICIENTFUNDSERRORMESSAGE);
            }

            bool isConfirmed = await this.ShowConfirmationDialogAsync();
            if (!isConfirmed)
            {
                return;
            }

            await this.PurchaseGamesAsync();
            if (this.LastEarnedPoints > ThresholdForNotEarningPoints)
            {
                // Store the points in App resources for PointsShopPage to access
                try
                {
                    Application.Current.Resources[ResourceKeys.RecentEarnedPoints] = this.LastEarnedPoints;
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine($"Error storing points: {exception.Message}");
                }

                await this.ShowPointsEarnedDialogAsync(this.LastEarnedPoints);
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task LoadGamesAsync()
    {
        var games = await this.cartService.GetCartGamesAsync();
        System.Diagnostics.Debug.WriteLine($"Number of games in cart: {games.Count}");
        foreach (var game in games)
        {
            System.Diagnostics.Debug.WriteLine($"GameId: {game.GameId}");
            this.CartGames.Add(game);
        }

        this.UpdateTotalPrice();
    }

    private void UpdateTotalPrice()
    {
        this.TotalPrice = (decimal)this.CartGames.Sum(game => (double)game.Price);
    }

    private async System.Threading.Tasks.Task ShowDialog(string title, string message)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = ConfirmationDialogStrings.OKBUTTONTEXT,
            XamlRoot = App.MainWindow.Content.XamlRoot,
        };

        await dialog.ShowAsync();
    }

    private async Task<bool> ShowConfirmationDialogAsync()
    {
        ContentDialog confirmDialog = new ContentDialog
        {
            Title = ConfirmationDialogStrings.CONFIRMPURCHASETITLE,
            Content = ConfirmationDialogStrings.CONFIRMPURCHASEASSURANCE,
            PrimaryButtonText = ConfirmationDialogStrings.YESBUTTONTEXT,
            CloseButtonText = ConfirmationDialogStrings.NOBUTTONTEXT,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = App.MainWindow.Content.XamlRoot,
        };
        ContentDialogResult result = await confirmDialog.ShowAsync();

        return result == ContentDialogResult.Primary;
    }

    private async Task ShowPointsEarnedDialogAsync(int pointsEarned)
    {
        ContentDialog pointsDialog = new ContentDialog
        {
            Title = ConfirmationDialogStrings.POINTSEARNEDTITLE,
            Content = string.Format(ConfirmationDialogStrings.POINTSEARNEDMESSAGE, pointsEarned),
            CloseButtonText = ConfirmationDialogStrings.OKBUTTONTEXT,
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.MainWindow.Content.XamlRoot,
        };

        await pointsDialog.ShowAsync();
    }
}