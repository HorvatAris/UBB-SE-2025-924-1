// <copyright file="CreditCardPaymentPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using SteamStore.Services.Interfaces;
    using SteamStore.ViewModels;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    public sealed partial class CreditCardPaymentPage : Page
    {
        public CreditCardPaymentPage(ICartService cartService, IUserGameService userGameService)
        {
            this.InitializeComponent();
            this.ViewModel = new CreditCardPaymentViewModel(cartService, userGameService);
            this.DataContext = this.ViewModel;
        }

        private CreditCardPaymentViewModel ViewModel { get; }

        private async void ProcessPaymentButton_Click(object processPaymentButton, RoutedEventArgs processPaymentEventArgument)
        {
            if (this.Parent is Frame frame)
            {
                await this.ViewModel.ProcessPaymentAsync(frame);
            }
        }

        private void NotificationDialog_Opened(ContentDialog notificationDialog, ContentDialogOpenedEventArgs argumentsForOpenedDialog)
        {
        }
    }
}
