// <copyright file="MarketplaceViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;
    using Steampunks.Services.MarketplaceService;

    /// <summary>
    /// ViewModel for the Marketplace Page.
    /// </summary>
    public partial class MarketplaceViewModel : INotifyPropertyChanged
    {
        private readonly IMarketplaceService marketplaceService;
        private ObservableCollection<Item> items;
        private string searchText;
        private string selectedGame;
        private string selectedType;
        private string selectedRarity;
        private List<Item> allCurrentItems;
        private Item selectedItem;
        private User currentUser;
        private ObservableCollection<User> availableUsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketplaceViewModel"/> class.
        /// </summary>
        /// <param name="marketplaceService"> Marketplace service used for the ViewModel. </param>
        public MarketplaceViewModel(IMarketplaceService marketplaceService)
        {
            this.marketplaceService = marketplaceService;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the AvailableGames ObservableCollection.
        /// </summary>
        public ObservableCollection<string> AvailableGames { get; set; }

        /// <summary>
        /// Gets or sets the AvailableTypes ObservableCollection.
        /// </summary>
        public ObservableCollection<string> AvailableTypes { get; set; }

        /// <summary>
        /// Gets or sets the AvailableRarities ObservableCollection.
        /// </summary>
        public ObservableCollection<string> AvailableRarities { get; set; }

        /// <summary>
        /// Gets or sets the Items ObservableCollection.
        /// </summary>
        public ObservableCollection<Item> Items
        {
            get => this.items;
            set
            {
                this.items = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the SearchText.
        /// </summary>
        public string SearchText
        {
            get => this.searchText;
            set
            {
                this.searchText = value;
                this.FilterItems();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the SelectedGame.
        /// </summary>
        public string SelectedGame
        {
            get => this.selectedGame;
            set
            {
                this.selectedGame = value;
                this.FilterItems();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the SelectedType.
        /// </summary>
        public string SelectedType
        {
            get => this.selectedType;
            set
            {
                this.selectedType = value;
                this.FilterItems();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the SelectedRarity.
        /// </summary>
        public string SelectedRarity
        {
            get => this.selectedRarity;
            set
            {
                this.selectedRarity = value;
                this.FilterItems();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the SelectedItem.
        /// </summary>
        public Item SelectedItem
        {
            get => this.selectedItem;
            set
            {
                if (this.selectedItem != value)
                {
                    this.selectedItem = value;
                    this.OnPropertyChanged(nameof(this.SelectedItem));
                    this.OnPropertyChanged(nameof(this.CanBuyItem));
                }
            }
        }

        /// <summary>
        /// Gets or sets the CurrentUser.
        /// </summary>
        public User CurrentUser
        {
            get => this.currentUser;
            set
            {
                if (this.currentUser != value)
                {
                    this.currentUser = value;
                    this.marketplaceService.SetCurrentUser(value);
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(this.CanBuyItem));
                }
            }
        }

        /// <summary>
        /// Gets or sets the AvailableUsers ObservableCollection.
        /// </summary>
        public ObservableCollection<User> AvailableUsers
        {
            get => this.availableUsers;
            set
            {
                this.availableUsers = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item selected can be bought (if it is not null, listed and if
        /// the current user is not null).
        /// </summary>
        public bool CanBuyItem => this.SelectedItem != null && this.SelectedItem.IsListed && this.CurrentUser != null;

        /// <summary>
        /// Handles the purchase of an item by removing former ownership, adding the item to the new owner's inventory,
        /// and marking the item as unlisted.
        /// </summary>
        /// <returns> True upon successful purchase. </returns>
        /// <exception cref="InvalidOperationException"> Thrown when either the selected item or current user is null,
        /// and if the selected item is not listed for purchase. </exception>
        public async Task<bool> BuyItemAsync()
        {
            if (this.SelectedItem == null || !this.SelectedItem.IsListed || this.CurrentUser == null)
            {
                throw new InvalidOperationException("Cannot buy item: Invalid state");
            }

            try
            {
                bool success = await this.marketplaceService.BuyItemAsync(this.SelectedItem);
                if (success)
                {
                    // Refresh the items list.
                    await this.LoadItemsAsync();
                    return true;
                }

                throw new InvalidOperationException("Failed to buy item");
            }
            catch (InvalidOperationException)
            {
                // Re-throw specific error messages.
                throw;
            }
            catch (Exception buyingItemException)
            {
                System.Diagnostics.Debug.WriteLine($"Error buying item: {buyingItemException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {buyingItemException.StackTrace}");
                throw new InvalidOperationException("An error occurred while buying the item. Please try again.");
            }
        }

        /// <summary>
        /// Initializes the ViewModel by asynchronously loading the items and users, as well as initializing the ObservableCollections.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        internal async Task InitializeViewModelAsync()
        {
            await this.LoadUsersAsync();
            await this.LoadItemsAsync();
            this.InitializeCollections();
        }

        private async Task LoadUsersAsync()
        {
            var users = await this.marketplaceService.GetAllUsersAsync();
            this.AvailableUsers = new ObservableCollection<User>(users);
            this.CurrentUser = this.marketplaceService.GetCurrentUser();
        }

        private async Task LoadItemsAsync()
        {
            var allItems = await this.marketplaceService.GetAllListingsAsync();
            this.allCurrentItems = allItems;
            this.Items = new ObservableCollection<Item>(allItems);
        }

        private void InitializeCollections()
        {
            var allItems = this.Items.ToList();
            this.AvailableGames = new ObservableCollection<string>(allItems.Select(item => item.Game.GameTitle).Distinct());
            this.AvailableTypes = new ObservableCollection<string>(allItems.Select(item => item.ItemName.Split('|').First().Trim()).Distinct());
            this.AvailableRarities = new ObservableCollection<string>(new[] { "Common", "Uncommon", "Rare", "Epic", "Legendary" });
        }

        private void FilterItems()
        {
            var filteredItems = this.allCurrentItems.AsQueryable();

            if (!string.IsNullOrEmpty(this.SearchText))
            {
                var searchTextLower = this.SearchText.ToLower();
                filteredItems = filteredItems.Where(item =>
                    item.ItemName.ToLower().Contains(searchTextLower) ||
                    item.Description.ToLower().Contains(searchTextLower));
            }

            if (!string.IsNullOrEmpty(this.SelectedGame))
            {
                filteredItems = filteredItems.Where(item => item.Game.GameTitle == this.SelectedGame);
            }

            if (!string.IsNullOrEmpty(this.SelectedType))
            {
                filteredItems = filteredItems.Where(item =>
                    item.ItemName.IndexOf('|') > 0
                        ? item.ItemName.Substring(0, item.ItemName.IndexOf('|')).Trim() == this.SelectedType
                        : item.ItemName.Trim() == this.SelectedType);
            }

            this.Items = new ObservableCollection<Item>(filteredItems);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
