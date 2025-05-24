// <copyright file="WishListViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.UI.Xaml.Controls;
    using SteamHub.ApiContract.Constants;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.Pages;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.ApiContract.Models.UsersGames;
    using SteamHub.ApiContract.Models.User;

    public class WishListViewModel : INotifyPropertyChanged
    {
        private readonly IUserGameService userGameService;
        private readonly IGameService gameService;
        private readonly ICartService cartService;
        private ObservableCollection<Game> wishListGames = new ObservableCollection<Game>();
        private string searchText = WishListSearchStrings.INITIALSEARCHSTRING;

        private string selectedFilter;
        private string selectedSort;
        private IUserDetails user;

        public WishListViewModel(IUserGameService userGameService, IGameService gameService, ICartService cartService)
        {
            this.userGameService = userGameService;
            this.gameService = gameService;
            this.cartService = cartService;
            this.wishListGames = new ObservableCollection<Game>();
            this.user = this.userGameService.GetUser();
            this.RemoveFromWishlistCommand = new RelayCommand<Game>(async (game) => await this.ConfirmAndRemoveFromWishlist(game));
            this.LoadWishListGames(user.UserId);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string SelectedFilter
        {
            get => this.selectedFilter;
            set
            {
                this.selectedFilter = value;
                this.OnPropertyChanged();
                this.HandleFilterChange();
            }
        }

        public string SelectedSort
        {
            get => this.selectedSort;
            set
            {
                this.selectedSort = value;
                this.OnPropertyChanged();
                this.HandleSortChange();
            }
        }

        public ObservableCollection<Game> WishListGames
        {
            get => this.wishListGames;
            set
            {
                this.wishListGames = value;
                this.OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => this.searchText;
            set
            {
                this.searchText = value;
                this.OnPropertyChanged();
                this.HandleSearchWishListGames();
            }
        }

        // Expose services for navigation
        public IGameService GameService => this.gameService;

        public ICartService CartService => this.cartService;

        public IUserGameService UserGameService => this.userGameService;

        public ICommand RemoveFromWishlistCommand { get; }

        public ICommand ViewDetailsCommand { get; }

        public ICommand BackCommand { get; }

        public List<string> FilterOptions { get; } = new ()
    {
        WishListSearchStrings.FILTERALL, WishListSearchStrings.FILTEROVERWHELMINGLYPOSITIVE, WishListSearchStrings.FILTERVERYPOSITIVE,
        WishListSearchStrings.FILTERMIXED, WishListSearchStrings.FILTERNEGATIVE,
    };

        public List<string> SortOptions { get; } = new ()
    {
        WishListSearchStrings.SORTPRICEASCENDING, WishListSearchStrings.SORTPRICEDESCENDING, WishListSearchStrings.SORTRATINGDESCENDING, WishListSearchStrings.SORTDISCOUNTDESCENDING,
    };

        public async Task HandleSearchWishListGames()
        {
            if (string.IsNullOrWhiteSpace(this.SearchText))
            {
                this.LoadWishListGames(this.user.UserId);
                return;
            }

            try
            {
                var games = await this.userGameService.SearchWishListByNameAsync(this.SearchText);
                this.WishListGames = new ObservableCollection<Game>(games);
            }
            catch (Exception exception)
            {
                // Handle error appropriately
                Console.WriteLine($"Error searching wishlist games: {exception.Message}");
            }
        }

        public async Task FilterWishListGames(string criteria)
        {
            try
            {
                var games = await this.userGameService.FilterWishListGamesAsync(criteria);
                this.WishListGames = new ObservableCollection<Game>(games);
            }
            catch (Exception exception)
            {
                // Handle error appropriately
                Console.WriteLine($"Error filtering wishlist games: {exception.Message}");
            }
        }

        public async Task SortWishListGames(string criteria, bool ascending)
        {
            try
            {
                var games = await this.userGameService.SortWishListGamesAsync(criteria, ascending);
                this.WishListGames = new ObservableCollection<Game>(games);
            }
            catch (Exception exception)
            {
                // Handle error appropriately
                Console.WriteLine($"Error sorting wishlist games: {exception.Message}");
            }
        }

        public async Task RemoveFromWishlist(Game game)
        {
            var gameRequest = new UserGameRequest
            {
                UserId = this.user.UserId,
                GameId = game.GameId
            };
            try
            {
                await this.userGameService.RemoveGameFromWishlistAsync(gameRequest);
                this.WishListGames.Remove(game);
            }
            catch (Exception exception)
            {
                // Handle error appropriately
                Console.WriteLine($"Error removing game from wishlist: {exception.Message}");
            }
        }

        public void BackToHomePage(Frame frame)
        {
            HomePage homePage = new HomePage(this.GameService, this.CartService, this.UserGameService);
            frame.Content = homePage;
        }

        public void ViewGameDetails(Frame frame, Game game)
        {
            GamePage gamePage = new GamePage(this.GameService, this.CartService, this.UserGameService, game);
            frame.Content = gamePage;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task ConfirmAndRemoveFromWishlist(Game game)
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = ConfirmationDialogStrings.CONFIRMREMOVALTITLE,
                    Content = string.Format(ConfirmationDialogStrings.CONFIRMREMOVALMESSAGE, game.GameTitle),
                    PrimaryButtonText = ConfirmationDialogStrings.YESBUTTONTEXT,
                    CloseButtonText = ConfirmationDialogStrings.NOBUTTONTEXT,
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = App.MainWindow.Content.XamlRoot,
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    this.RemoveFromWishlist(game);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error showing dialog: {exception.Message}");
            }
        }

        private void HandleFilterChange()
        {
            string criteria = this.SelectedFilter switch
            {
                WishListSearchStrings.FILTEROVERWHELMINGLYPOSITIVE => WishListSearchStrings.OVERWHELMINGLYPOSITIVE,
                WishListSearchStrings.FILTERVERYPOSITIVE => WishListSearchStrings.VERYPOSITIVE,
                WishListSearchStrings.FILTERMIXED => WishListSearchStrings.MIXED,
                WishListSearchStrings.FILTERNEGATIVE => WishListSearchStrings.NEGATIVE,
                _ => WishListSearchStrings.ALL
            };

            this.FilterWishListGames(criteria);
        }

        private void HandleSortChange()
        {
            switch (this.SelectedSort)
            {
                case WishListSearchStrings.SORTPRICEASCENDING:
                    this.SortWishListGames(FilterCriteria.PRICE, true); break;
                case WishListSearchStrings.SORTPRICEDESCENDING:
                    this.SortWishListGames(FilterCriteria.PRICE, false); break;
                case WishListSearchStrings.SORTRATINGDESCENDING:
                    this.SortWishListGames(FilterCriteria.RATING, false); break;
                case WishListSearchStrings.SORTDISCOUNTDESCENDING:
                    this.SortWishListGames(FilterCriteria.DISCOUNT, false); break;
            }
        }

        private async Task LoadWishListGames(int userId)
        {
            try
            {
                var games = await this.userGameService.GetWishListGamesAsync(userId);
                this.WishListGames = new ObservableCollection<Game>(games);
            }
            catch (Exception exception)
            {
                // Handle error appropriately
                Console.WriteLine($"Error loading wishlist games: {exception.Message}");
            }
        }
    }
}