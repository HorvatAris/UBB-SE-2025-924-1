// <copyright file="CartPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SteamHub.Services.Interfaces;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CartPage : Page
    {
        private const int EmptyGamesCounter = 0;
        private CartViewModel viewModel;

        public CartPage(ICartService cartService, IUserGameService userGameService)
        {
            this.InitializeComponent();
            this.viewModel = new CartViewModel(cartService, userGameService);
            this.DataContext = this.viewModel;
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs checkoutEventArgument)
        {
            if (this.viewModel.CartGames.Count > EmptyGamesCounter)
            {
                if (this.Parent is Frame frame)
                {
                    this.viewModel.ChangeToPaymentPageAsync(frame);
                }
            }
        }
    }
}