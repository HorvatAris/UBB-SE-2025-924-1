namespace SteamStore
{
    using System;
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

    public sealed partial class MainWindow : Window
    {
        private GameService gameService;
        private CartService cartService;
        private UserGameService userGameService;
        private DeveloperService developerService;
        private PointShopService pointShopService;
        private InventoryService inventoryService;
        private MarketplaceService marketplaceService;
        private TradeService tradeService;
        private UserService userService;
        public User user;

        public MainWindow()
        {
            this.InitializeComponent();

            //initiate the user
            // this will need to be changed when we conenct with a database query to get the user
            User loggedInUser = new User(1, "John Doe", "johnyDoe@gmail.com", 999999.99f, 6000f, User.Role.Developer);

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

            var tradeService = new TradeService(itemTradeServiceProxy, loggedInUser, itemTradeDetailServiceProxy,userServiceProxy, gameServiceProxy,itemServiceProxy,userInventoryServiceProxy);
            this.tradeService = tradeService;

            var userService = new UserService(userServiceProxy);
            this.userService = userService;
            var trade = new ItemTrade(
                sourceUser: loggedInUser,
                destinationUser: new User { UserId = 2 }, // the user you want to trade with
                gameOfTrade: new Game { GameId = 1 }, // the game the items belong to
                description: "Test trade from user 1 to user 2"
            );
            trade.SetTradeId(1); // Set the trade ID to 1 for testing purposes

            // Add source user items
            trade.AddSourceUserItem(new Item { ItemId = 1 });
            trade.AddSourceUserItem(new Item { ItemId = 2 });

            // Add destination user items
            trade.AddDestinationUserItem(new Item { ItemId = 3 });
            this.marketplaceService = new MarketplaceService
            {
                userInventoryServiceProxy = userInventoryServiceProxy,
                gameServiceProxy = gameServiceProxy,
                userServiceProxy = userServiceProxy,
                itemServiceProxy = itemServiceProxy,
                User = loggedInUser,
            };

            //var cartServiceProxy = RestService.For<ICartServiceProxy>(httpClient);
            var tagServiceProxy = RestService.For<ITagServiceProxy>(httpClient);

            //var userServiceProxy = RestService.For<IUserServiceProxy>(httpClient);
            var userGameServiceProxy = RestService.For<IUserGameServiceProxy>(httpClient);

            pointShopService = new PointShopService(
                pointShopServiceProxy,
                userPointShopInventoryServiceProxy,
                userServiceProxy,
                loggedInUser);


            this.inventoryService = new InventoryService(userInventoryServiceProxy, itemServiceProxy, gameServiceProxy, user);

            gameService = new GameService { GameServiceProxy = gameServiceProxy, TagServiceProxy = tagServiceProxy };

            cartService = new CartService(userGameServiceProxy, loggedInUser,gameServiceProxy);
            var userGameRepository = new UserGameRepository(dataLink, loggedInUser);
            userGameService = new UserGameService(userGameServiceProxy, gameServiceProxy, tagServiceProxy, loggedInUser);

            developerService = new DeveloperService
            (gameServiceProxy, tagServiceProxy, userGameServiceProxy, userServiceProxy, itemServiceProxy, itemTradeDetailServiceProxy, loggedInUser);

            if (ContentFrame == null)
            {
                throw new Exception("ContentFrame is not initialized.");
            }
            _ = Task.Run(async () =>
            {
                try
                {
                   // await tradeService.AddItemTradeAsync(trade);
                    //await tradeService.GetActiveTradesAsync(1);
                    //await tradeService.UpdateItemTradeAsync(trade);
                   // await tradeService.GetUserInventoryAsync(1);
                   await tradeService.GetUserInventoryAsync(2);
                   Debug.WriteLine("Trade created successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error creating trade: {ex.Message}");
                }
            });
            ContentFrame.Content = new HomePage(gameService, cartService, userGameService);
        }

        public void ResetToHomePage()
        {
            ContentFrame.Content = new HomePage(gameService, cartService, userGameService);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                var tag = args.SelectedItemContainer.Tag.ToString();
                switch (tag)
                {
                    case "HomePage":
                        ContentFrame.Content = new HomePage(gameService, cartService, userGameService);
                        break;
                    case "CartPage":
                        ContentFrame.Content = new CartPage(cartService, userGameService);
                        break;
                    case "PointsShopPage":
                        ContentFrame.Content = new PointsShopPage(pointShopService);
                        break;
                    case "WishlistPage":
                        ContentFrame.Content = new WishListView(userGameService, gameService, cartService);
                        break;
                    case "DeveloperModePage":
                        ContentFrame.Content = new DeveloperModePage(developerService);
                        break;
                    case "inventory":
                        ContentFrame.Content = new InventoryPage(inventoryService);
                        break;
                    case "marketplace":
                        ContentFrame.Content = new MarketplacePage(this.marketplaceService);
                        break;
                    case "trading":
                        ContentFrame.Content = new TradingPage(tradeService,userService,gameService);
                        break;
                }
            }

            if (NavView != null)
            {
                // Deselect the NavigationViewItem when moving to a non-menu page
                NavView.SelectedItem = null;
            }
        }
    }
}