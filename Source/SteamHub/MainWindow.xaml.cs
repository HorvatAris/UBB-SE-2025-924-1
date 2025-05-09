// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using SteamHub.Models;
using SteamHub.Pages;
using SteamHub.Proxies;
using SteamHub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Refit;
using SteamHub.ApiContract.Models.User;

namespace SteamHub
{
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

            var users = new List<User>
            {
                new User
                {
                    UserId = 3,
                    Email = "john.chen@thatgamecompany.com",
                    PointsBalance = 5000,
                    UserName = "JohnC",
                    UserRole = User.Role.Developer,
                    WalletBalance = 390,
                },

                new User
                {
                    UserId = 4,
                    Email = "alice.johnson@example.com",
                    PointsBalance = 6000,
                    UserName = "AliceJ",
                    UserRole = User.Role.User,
                    WalletBalance = 78,
                },

                new User
                {
                    UserId = 5,
                    Email = "liam.garcia@example.com",
                    PointsBalance = 7000,
                    UserName = "LiamG",
                    UserRole = User.Role.User,
                    WalletBalance = 55,
                },

                new User
                {
                    UserId = 7,
                    Email = "noah.smith@example.com",
                    PointsBalance = 4000,
                    UserName = "NoahS",
                    UserRole = User.Role.User,
                    WalletBalance = 33,
                },
            };

            User loggedInUser = users[2];

            // Assign to the class field so it can be used in navigation
            this.user = loggedInUser;
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7241"),
            };

            var pointShopRepository = new PointShopItemRepositoryProxy();
            var userPointShopInventoryRepository = new UserPointShopItemInventoryRepositoryProxy();
            var gameRepository = new GameRepositoryProxy();
            var userRepository = new UserRepositoryProxy();
            var itemRepository = new ItemRepositoryProxy();
            var itemTradeRepository = new ItemTradeRepositoryProxy();
            var itemTradeDetailsRepository = new ItemTradeDetailsRepositoryProxy();
            var userInventoryRepository = new UserInventoryRepositoryProxy();
            var tagRepository = new TagRepositoryProxy();
            var userGamesRepository = new UserGamesRepositoryProxy();


            var tradeService = new TradeService(itemTradeRepository, loggedInUser, itemTradeDetailsRepository, userRepository, gameRepository, itemRepository, userInventoryRepository);
            this.tradeService = tradeService;

            var userService = new UserService(userRepository);
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
                UserInventoryRepository = userInventoryRepository,
                GameRepository = gameRepository,
                UserRepository = userRepository,
                ItemRepository = itemRepository,
                User = loggedInUser,
            };

            this.pointShopService = new PointShopService(
                pointShopRepository,
                userPointShopInventoryRepository,
                userRepository,
                loggedInUser);

            this.inventoryService = new InventoryService(userInventoryRepository, itemRepository, gameRepository, this.user);

            this.gameService = new GameService { GameRepository = gameRepository, TagRepository = tagRepository };

            this.cartService = new CartService(userGamesRepository, loggedInUser, gameRepository);

            this.userGameService = new UserGameService(userGamesRepository, gameRepository, tagRepository, loggedInUser);

            this.developerService = new DeveloperService(
            gameRepository, tagRepository, userGamesRepository, userRepository, itemRepository, itemTradeDetailsRepository, loggedInUser);

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