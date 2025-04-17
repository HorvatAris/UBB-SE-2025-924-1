// <copyright file="PaypalPaymentPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text.RegularExpressions;
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

    public sealed partial class PaypalPaymentPage : Page
    {
        private PaypalPaymentViewModel viewModel;

        public PaypalPaymentPage(ICartService cartService, IUserGameService userGameService)
        {
            this.InitializeComponent();
            this.viewModel = new PaypalPaymentViewModel(cartService, userGameService);
            this.DataContext = this.viewModel;
        }

        private async void ValidateButton_Click(object validateButton, RoutedEventArgs validateClickEventArgument)
        {
            if (this.Parent is Frame frame)
            {
                await this.viewModel.ValidatePayment(frame);
            }
        }

        private void NotificationDialog_Opened(ContentDialog notificationDialog, ContentDialogOpenedEventArgs dialogOpenedEventArguments)
        {
        }
    }
}