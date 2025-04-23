// <copyright file="NavigationViewService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// A service that integrates the NavigationView UI with navigation logic.
    /// </summary>
    public class NavigationViewService : INavigationViewService
    {
        private readonly INavigationService navigationService;
        private NavigationView? navigationView;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewService"/> class.
        /// </summary>
        /// <param name="navigationService">The service used for navigation.</param>
        public NavigationViewService(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        /// <inheritdoc/>
        public INavigationService NavigationService => this.navigationService;

        /// <inheritdoc/>
        public NavigationView? NavigationView
        {
            get => this.navigationView;
            set
            {
                this.UnregisterEvents();
                this.navigationView = value;
                this.RegisterEvents();
            }
        }

        /// <inheritdoc/>
        public void Initialize(NavigationView navigationView)
        {
            this.NavigationView = navigationView;
        }

        /// <inheritdoc/>
        public void UnregisterEvents()
        {
            if (this.navigationView != null)
            {
                this.navigationView.ItemInvoked -= this.NavigationView_ItemInvoked;
                this.navigationView.BackRequested -= this.NavigationView_BackRequested;
            }
        }

        private void RegisterEvents()
        {
            if (this.navigationView != null)
            {
                this.navigationView.ItemInvoked += this.NavigationView_ItemInvoked;
                this.navigationView.BackRequested += this.NavigationView_BackRequested;
            }
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                this.navigationService.NavigateTo("settings");
                return;
            }

            var selectedItem = args.InvokedItemContainer as NavigationViewItem;
            if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
            {
                this.navigationService.NavigateTo(pageKey);
            }
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            this.navigationService.GoBack();
        }
    }
}