﻿// <copyright file="CartViewModel.cs" company="PlaceholderCompany">
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
using SteamStore;
using SteamStore.Constants;
using SteamStore.Pages;
using SteamStore.Services.Interfaces;
using SteamStore.ViewModels;

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
        this.LoadGames();

        // Initialize commands
        this.RemoveGameCommand = new RelayCommand<Game>(this.RemoveGameFromCart);
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

    public void RemoveGameFromCart(Game game)
    {
        this.cartService.RemoveGameFromCart(game);
        this.CartGames.Remove(game);
        this.UpdateTotalPrice();
        this.OnPropertyChanged(nameof(this.CartGames));
    }

    public void PurchaseGames()
    {
        this.userGameService.PurchaseGames(this.CartGames.ToList());

        // Get the points earned from the user game service
        this.LastEarnedPoints = this.userGameService.LastEarnedPoints;

        this.cartService.RemoveGamesFromCart(this.CartGames.ToList());
        this.CartGames.Clear();
        this.UpdateTotalPrice();
    }

    public async void ChangeToPaymentPage(Frame frame)
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

            this.PurchaseGames();
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

    private void LoadGames()
    {
        var games = this.cartService.GetCartGames();
        foreach (var game in games)
        {
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
            CloseButtonText = DialogStrings.OKBUTTONTEXT,
            XamlRoot = App.MainWindow.Content.XamlRoot,
        };

        await dialog.ShowAsync();
    }

    private async Task<bool> ShowConfirmationDialogAsync()
    {
        ContentDialog confirmDialog = new ContentDialog
        {
            Title = DialogStrings.CONFIRMPURCHASETITLE,
            Content = DialogStrings.CONFIRMPURCHASEMESSAGE,
            PrimaryButtonText = DialogStrings.YESBUTTONTEXT,
            CloseButtonText = DialogStrings.NOBUTTONTEXT,
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
            Title = DialogStrings.POINTSEARNEDTITLE,
            Content = string.Format(DialogStrings.POINTSEARNEDMESSAGE, pointsEarned),
            CloseButtonText = DialogStrings.OKBUTTONTEXT,
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.MainWindow.Content.XamlRoot,
        };

        await pointsDialog.ShowAsync();
    }
}