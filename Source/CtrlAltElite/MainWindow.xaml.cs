using System.Net.Http;
using CtrlAltElite.ServiceProxies;
using Refit;

namespace SteamStore
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SteamStore.Pages;
    using SteamStore.Repositories;
    using SteamStore.Services;

    public sealed partial class MainWindow : Window
    {
        private GameService gameService;
        private CartService cartService;
        private UserGameService userGameService;
        private DeveloperService developerService;
        private PointShopService pointShopService;
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
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7241")
            };
            var pointShopRepository = new PointShopRepository(loggedInUser,dataLink);
            this.pointShopService = new PointShopService(pointShopRepository);
            var gameServiceProxy = RestService.For<IGameServiceProxy>(httpClient);
           // var tagRepository = new TagRepository(dataLink);
            var tagServiceProxy = RestService.For<ITagServiceProxy>(httpClient);

            gameService = new GameService { GameServiceProxy = gameServiceProxy, TagServiceProxy = tagServiceProxy };

            cartService = new CartService(new CartRepository(dataLink, loggedInUser));
            var userGameRepository = new UserGameRepository(dataLink, loggedInUser);
            userGameService = new UserGameService
            {
                UserGameRepository = userGameRepository,
                GameServiceProxy = gameServiceProxy,
                TagServiceProxy = tagServiceProxy,

            };

            developerService = new DeveloperService
            {
                // GameRepository = gameRepository,
                //TagRepository = tagRepository,
                TagServiceProxy = tagServiceProxy,
                UserGameRepository = userGameRepository,
                User = loggedInUser,
                GameServiceProxy = gameServiceProxy,
            };

            if (ContentFrame == null)
            {
                throw new Exception("ContentFrame is not initialized.");
            }

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