// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Proxies;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.ServiceProxies;
using SteamHub.Pages;
using SteamHub.Web;

namespace SteamHub
{
    /// <summary>
    /// Main window.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private User user;
        private GameServiceProxy gameService;
        private CartServiceProxy cartService;
        private UserGameServiceProxy userGameService;
        private DeveloperServiceProxy developerService;
        private PointShopServiceProxy pointShopService;
        private InventoryServiceProxy inventoryService;
        private MarketplaceServiceProxy marketplaceService;
        private TradeServiceProxy tradeService;
        private UserServiceProxy userService;

        public MainWindow()
        {
            this.InitializeComponent();

            // initiate the user
            // this will need to be changed when we conenct with a database query to get the user

            var users = new List<User>
            {
                new User
                {
                    UserId = 3,
                    Email = "john.chen@thatgamecompany.com",
                    PointsBalance = 5000,
                    UserName = "JohnC",
                    UserRole = UserRole.Developer,
                    WalletBalance = 390,
                },

                new User
                {
                    UserId = 4,
                    Email = "alice.johnson@example.com",
                    PointsBalance = 6000,
                    UserName = "AliceJ",
                    UserRole = UserRole.User,
                    WalletBalance = 78,
                },

                new User
                {
                    UserId = 5,
                    Email = "liam.garcia@example.com",
                    PointsBalance = 7000,
                    UserName = "LiamG",
                    UserRole = UserRole.User,
                    WalletBalance = 55,
                },

                new User
                {
                    UserId = 7,
                    Email = "noah.smith@example.com",
                    PointsBalance = 4000,
                    UserName = "NoahS",
                    UserRole = UserRole.User,
                    WalletBalance = 33,
                },
            };

            User loggedInUser = users[1];

            // Assign to the class field so it can be used in navigation
            this.user = loggedInUser;
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            };

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = configBuilder.Build();
            var services = new ServiceCollection();
            services.AddHttpClient("SteamHubApi", client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
    
            }).ConfigurePrimaryHttpMessageHandler(() => new NoSslCertificateValidationHandler());
            var provider = services.BuildServiceProvider();

            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

            this.tradeService = new TradeServiceProxy(httpClientFactory, loggedInUser);

            this.userService = new UserServiceProxy(httpClientFactory);

            this.marketplaceService = new MarketplaceServiceProxy(httpClientFactory, loggedInUser);

            this.pointShopService = new PointShopServiceProxy(httpClientFactory, loggedInUser);

            this.inventoryService = new InventoryServiceProxy(httpClientFactory,loggedInUser);

            this.gameService = new GameServiceProxy(httpClientFactory);

            this.cartService = new CartServiceProxy(httpClientFactory, loggedInUser);

            this.userGameService = new UserGameServiceProxy(httpClientFactory, loggedInUser);

            this.developerService = new DeveloperServiceProxy(httpClientFactory, loggedInUser);

            if (this.ContentFrame == null)
            {
                throw new Exception("ContentFrame is not initialized.");
            }

            this.ContentFrame.Content = new HomePage(this.gameService, this.cartService, this.userGameService);
        }

        public void ResetToHomePage()
        {
            this.ContentFrame.Content = new HomePage(this.gameService, this.cartService, this.userGameService);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                var tag = args.SelectedItemContainer.Tag.ToString();
                switch (tag)
                {
                    case "HomePage":
                        this.ContentFrame.Content = new HomePage(this.gameService, this.cartService, this.userGameService);
                        break;
                    case "CartPage":
                        this.ContentFrame.Content = new CartPage(this.cartService, this.userGameService);
                        break;
                    case "PointsShopPage":
                        this.ContentFrame.Content = new PointsShopPage(this.pointShopService);
                        break;
                    case "WishlistPage":
                        this.ContentFrame.Content = new WishListView(this.userGameService, this.gameService, this.cartService);
                        break;
                    case "DeveloperModePage":
                        this.ContentFrame.Content = new DeveloperModePage(this.developerService);
                        break;
                    case "inventory":
                        this.ContentFrame.Content = new InventoryPage(this.inventoryService);
                        break;
                    case "marketplace":
                        this.ContentFrame.Content = new MarketplacePage(this.marketplaceService);
                        break;
                    case "trading":
                        this.ContentFrame.Content = new TradingPage(this.tradeService, this.userService, this.gameService);
                        break;
                }
            }

            if (this.NavView != null)
            {
                // Deselect the NavigationViewItem when moving to a non-menu page
                this.NavView.SelectedItem = null;
            }
        }
    }
}