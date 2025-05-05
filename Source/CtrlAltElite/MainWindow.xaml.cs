// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.Pages;
    using CtrlAltElite.ServiceProxies;
    using CtrlAltElite.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Refit;
    using SteamStore.Pages;
    using SteamStore.Services;

    /// <summary>
    /// Main window.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private User user;
        private GameService gameService;
        private CartService cartService;
        private UserGameService userGameService;
        private DeveloperService developerService;
        private PointShopService pointShopService;
        private InventoryService inventoryService;
        private MarketplaceService marketplaceService;
        private TradeService tradeService;
        private UserService userService;

        public MainWindow()
        {
            this.InitializeComponent();

            // initiate the user
            // this will need to be changed when we conenct with a database query to get the user
            User loggedInUser = new User
            {
                UserId = 5,
                Email = "liam.garcia@example.com",
                PointsBalance = 67,
                UserName = "LiamG",
                UserRole = User.Role.User,
                WalletBalance = 55
            };

            // Assign to the class field so it can be used in navigation
            this.user = loggedInUser;

            var dataLink = new DataLink(
                new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build());

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7241"),
            };

            var pointShopServiceProxy = RestService.For<IPointShopItemServiceProxy>(httpClient);
            var userPointShopInventoryServiceProxy = RestService.For<IUserPointShopItemInventoryServiceProxy>(httpClient);
            var gameServiceProxy = RestService.For<IGameServiceProxy>(httpClient);
            var userServiceProxy = RestService.For<IUserServiceProxy>(httpClient);
            var itemServiceProxy = RestService.For<IItemServiceProxy>(httpClient);
            var itemTradeServiceProxy = RestService.For<IITemTradeServiceProxy>(httpClient);
            var itemTradeDetailServiceProxy = RestService.For<IItemTradeDetailServiceProxy>(httpClient);
            var userInventoryServiceProxy = RestService.For<IUserInventoryServiceProxy>(httpClient);

            var tradeService = new TradeService(itemTradeServiceProxy, loggedInUser, itemTradeDetailServiceProxy, userServiceProxy, gameServiceProxy, itemServiceProxy, userInventoryServiceProxy);
            this.tradeService = tradeService;

            var userService = new UserService(userServiceProxy);
            this.userService = userService;
            var trade = new ItemTrade
            {
                TradeId = 1,
                SourceUser = loggedInUser,
                DestinationUser = new User { UserId = 2 },
                GameOfTrade = new Game { GameId = 1 },
                TradeDescription = "Test trade from user 1 to user 2",
            };

            // Add source user items
            trade.SourceUserItems.Add(new Item { ItemId = 1 });
            trade.SourceUserItems.Add(new Item { ItemId = 2 });

            // Add destination user items
            trade.DestinationUserItems.Add(new Item { ItemId = 3 });
            this.marketplaceService = new MarketplaceService
            {
                UserInventoryServiceProxy = userInventoryServiceProxy,
                GameServiceProxy = gameServiceProxy,
                UserServiceProxy = userServiceProxy,
                ItemServiceProxy = itemServiceProxy,
                User = loggedInUser,
            };

            // var cartServiceProxy = RestService.For<ICartServiceProxy>(httpClient);
            var tagServiceProxy = RestService.For<ITagServiceProxy>(httpClient);

            // var userServiceProxy = RestService.For<IUserServiceProxy>(httpClient);
            var userGameServiceProxy = RestService.For<IUserGameServiceProxy>(httpClient);

            this.pointShopService = new PointShopService(
                pointShopServiceProxy,
                userPointShopInventoryServiceProxy,
                userServiceProxy,
                loggedInUser);

            this.inventoryService = new InventoryService(userInventoryServiceProxy, itemServiceProxy, gameServiceProxy, this.user);

            this.gameService = new GameService { GameServiceProxy = gameServiceProxy, TagServiceProxy = tagServiceProxy };

            this.cartService = new CartService(userGameServiceProxy, loggedInUser, gameServiceProxy);
            var userGameRepository = new UserGameRepository(dataLink, loggedInUser);
            this.userGameService = new UserGameService(userGameServiceProxy, gameServiceProxy, tagServiceProxy, loggedInUser);

            this.developerService = new DeveloperService(
            gameServiceProxy, tagServiceProxy, userGameServiceProxy, userServiceProxy, itemServiceProxy, itemTradeDetailServiceProxy, loggedInUser);

            if (this.ContentFrame == null)
            {
                throw new Exception("ContentFrame is not initialized.");
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    // await tradeService.AddItemTradeAsync(trade);
                    // await tradeService.GetActiveTradesAsync(1);
                    // await tradeService.UpdateItemTradeAsync(trade);
                    // await tradeService.GetUserInventoryAsync(1);
                    await tradeService.GetUserInventoryAsync(2);
                    Debug.WriteLine("Trade created successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error creating trade: {ex.Message}");
                }
            });
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