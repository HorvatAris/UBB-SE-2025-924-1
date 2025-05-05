using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.ViewModels
{
    using CtrlAltElite.Models;
    using CtrlAltElite.Services.Interfaces;
    using SteamStore.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public partial class MarketplaceViewModel : INotifyPropertyChanged
    {
        private IMarketplaceService marketplaceService;
        private ObservableCollection<Item> items;
        private string searchText;
        private string selectedGame;
        private string selectedType;
        private string selectedRarity;
        private List<Item> allCurrentItems;
        private Item selectedItem;
        private User currentUser;
        private ObservableCollection<User> availableUsers;

        public MarketplaceViewModel(IMarketplaceService marketplaceService)
        {
            this.marketplaceService = marketplaceService;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<string> _availableGames;

        public ObservableCollection<string> AvailableGames
        {
            get => _availableGames;
            set
            {
                if (_availableGames != value)
                {
                    _availableGames = value;
                    OnPropertyChanged(nameof(AvailableGames)); // Notify the UI about changes
                }
            }
        }

        private ObservableCollection<string> _availableTypes;

        public ObservableCollection<string> AvailableTypes
        {
            get => _availableTypes;
            set
            {
                if (_availableTypes != value)
                {
                    _availableTypes = value;
                    OnPropertyChanged(nameof(AvailableTypes)); // Notify the UI about changes
                }
            }
        }

        private ObservableCollection<string> _availableRarities;

        public ObservableCollection<string> AvailableRarities
        {
            get => _availableRarities;
            set
            {
                if (_availableRarities != value)
                {
                    _availableRarities = value;
                    OnPropertyChanged(nameof(AvailableRarities)); // Notify the UI about changes
                }
            }
        }

        public ObservableCollection<Item> Items
        {
            get => this.items;
            set
            {
                this.items = value;
                this.OnPropertyChanged();
            }
        }

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

        public User CurrentUser
        {
            get => this.currentUser;
            set
            {
                if (this.currentUser != value)
                {
                    this.currentUser = value;
                    this.marketplaceService.User = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(this.CanBuyItem));
                }
            }
        }

        public ObservableCollection<User> AvailableUsers
        {
            get => this.availableUsers;
            set
            {
                this.availableUsers = value;
                this.OnPropertyChanged();
            }
        }

        public bool CanBuyItem => this.SelectedItem != null && this.SelectedItem.IsListed && this.CurrentUser != null;

        public async Task<bool> BuyItemAsync()
        {
            if (this.SelectedItem == null || !this.SelectedItem.IsListed || this.CurrentUser == null)
            {
                throw new InvalidOperationException("Cannot buy item: Invalid state");
            }

            try
            {
                bool success = await this.marketplaceService.BuyItemAsync(this.SelectedItem, this.CurrentUser.UserId);
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
            this.CurrentUser = this.marketplaceService.User;
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
            this.AvailableGames =
                new ObservableCollection<string>(allItems.Select(item => item.Game.GameTitle).Distinct());
            this.AvailableTypes = new ObservableCollection<string>(
                allItems.Select(item => item.ItemName.Split('|').First().Trim()).Distinct());
            this.AvailableRarities =
                new ObservableCollection<string>(new[] { "Common", "Uncommon", "Rare", "Epic", "Legendary" });
        }

        private void FilterItems()
        {
            var filteredItems = this.allCurrentItems.AsQueryable();

            if (!string.IsNullOrEmpty(this.SearchText))
            {
                var searchTextLower = this.SearchText.ToLower();
                filteredItems = filteredItems.Where(
                    item =>
                        item.ItemName.ToLower().Contains(searchTextLower) ||
                        item.Description.ToLower().Contains(searchTextLower));
            }

            if (!string.IsNullOrEmpty(this.SelectedGame))
            {
                filteredItems = filteredItems.Where(item => item.Game.GameTitle == this.SelectedGame);
            }

            if (!string.IsNullOrEmpty(this.SelectedType))
            {
                filteredItems = filteredItems.Where(
                    item =>
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