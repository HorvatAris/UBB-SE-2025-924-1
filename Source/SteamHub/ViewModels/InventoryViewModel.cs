// <copyright file="InventoryViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Services;
    using SteamHub.ApiContract.Services.Interfaces;

    public class InventoryViewModel : INotifyPropertyChanged
    {
        private IUserDetails user;
        private readonly IInventoryService inventoryService;
        private ObservableCollection<Item> inventoryItems;
        private ObservableCollection<Game> availableGames;
        private ObservableCollection<User> availableUsers;
        private Game selectedGame;
        private User selectedUser;
        private string searchText;
        private bool isUpdating;
        private Item selectedItem;

        public InventoryViewModel(IInventoryService inventoryS)
        {
            // this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            System.Diagnostics.Debug.WriteLine(inventoryS.ToString());
            this.inventoryService = inventoryS;
            this.inventoryItems = new ObservableCollection<Item>();
            this.availableGames = new ObservableCollection<Game>();
            this.availableUsers = new ObservableCollection<User>();
            this.user = this.inventoryService.GetAllUsers();

            // Load users and initialize data.

            // this.LoadUsersAsync().GetAwaiter().GetResult();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Item> InventoryItems
        {
            get => this.inventoryItems;
            private set
            {
                if (this.inventoryItems != value)
                {
                    this.inventoryItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Game> AvailableGames
        {
            get => this.availableGames;
            private set
            {
                if (this.availableGames != value)
                {
                    this.availableGames = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<User> AvailableUsers
        {
            get => this.availableUsers;
            private set
            {
                if (this.availableUsers != value)
                {
                    this.availableUsers = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Game SelectedGame
        {
            get => this.selectedGame;
            set
            {
                if (this.selectedGame != value)
                {
                    this.selectedGame = value;
                    this.OnPropertyChanged();

                    // Update the filtered inventory when the game filter changes.
                    _ = this.UpdateInventoryItemsAsync();
                }
            }
        }

        public User SelectedUser
        {
            get => this.selectedUser;
            set
            {
                if (this.selectedUser != value && !this.isUpdating)
                {
                    this.selectedUser = value;
                    this.OnPropertyChanged();

                    // When a user is selected, load their inventory.
                    if (this.selectedUser != null)
                    {
                        _ = this.LoadInventoryItemsAsync();
                    }
                }
            }
        }

        public string SearchText
        {
            get => this.searchText;
            set
            {
                if (this.searchText != value)
                {
                    this.searchText = value;
                    this.OnPropertyChanged();

                    // Update the filtered inventory when the search text changes.
                    this.UpdateAsyncVoid();
                }
            }
        }

        private async void updateAsyncVoid()
        {
            await UpdateInventoryItemsAsync();
        }

        public Item SelectedItem
        {
            get => this.selectedItem;
            set
            {
                if (this.selectedItem != value)
                {
                    this.selectedItem = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public async Task InitializeAsync()
        {
            await this.LoadUsersAsync();
        }

        public async Task LoadInventoryItemsAsync()
        {
            if (this.SelectedUser == null)
            {
                return;
            }

            try
            {
                this.isUpdating = true;

                // Retrieve filtered inventory based on the current game filter and search text.
                var filteredItems = await this.inventoryService.GetUserFilteredInventoryAsync(
                    this.SelectedUser.UserId,
                    this.SelectedGame,
                    this.SearchText);

                // Update the inventory items collection.
                this.InventoryItems.Clear();
                foreach (var item in filteredItems)
                {
                    this.InventoryItems.Add(item);
                }

                // Retrieve all inventory items to rebuild the games filter.
                var allItems = await this.inventoryService.GetUserInventoryAsync(this.SelectedUser.UserId);
                var availableGames = await this.inventoryService.GetAvailableGamesAsync(allItems,this.user.UserId);
                this.AvailableGames.Clear();
                foreach (var game in availableGames)
                {
                    this.AvailableGames.Add(game);
                }
            }
            catch (Exception loadingInventoryItemException)
            {
                // Log exception details as needed.
                System.Diagnostics.Debug.WriteLine($"Error loading inventory items: {loadingInventoryItemException.Message}");
                this.InventoryItems.Clear();
            }
            finally
            {
                this.isUpdating = false;
            }
        }

        public async Task<bool> SellItemAsync(Item selectedItem)
        {
            return await this.inventoryService.SellItemAsync(selectedItem);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void UpdateAsyncVoid()
        {
            await this.UpdateInventoryItemsAsync();
        }

        private async Task UpdateInventoryItemsAsync()
        {
            if (this.SelectedUser == null)
            {
                return;
            }

            try
            {
                this.isUpdating = true;

                var filteredItems = await this.inventoryService.GetUserFilteredInventoryAsync(
                    this.SelectedUser.UserId,
                    this.SelectedGame,
                    this.SearchText);

                this.InventoryItems.Clear();
                foreach (var item in filteredItems)
                {
                    this.InventoryItems.Add(item);
                }
            }
            catch (Exception updatingInventoryItemsException)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating inventory items: {updatingInventoryItemsException.Message}");
                this.InventoryItems.Clear();
            }
            finally
            {
                this.isUpdating = false;
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var user = new User(this.inventoryService.GetAllUsers());
                this.AvailableUsers.Clear();
                this.AvailableUsers.Add(user);
                this.SelectedUser = user;
            }
            catch (Exception loadingUsersException)
            {
                // Log exception details as needed.
                System.Diagnostics.Debug.WriteLine($"Error loading users: {loadingUsersException.Message}");
                this.AvailableUsers.Clear();
            }
        }

        /// <summary>
        /// Attempts to sell the specified item and returns a message indicating the result.
        /// </summary>
        /// <param name="item">The item to be sold.</param>
        /// <returns>A tuple indicating whether the sale was successful and the message to show.</returns>
        public async Task<(bool IsSuccess, string Message)> TrySellItemAsync(Item item)
        {
            bool success = await this.SellItemAsync(item);
            string message = success
                ? string.Format("{0} has been successfully listed for sale!", item.ItemName)
                : "Failed to sell the item. Please try again.";

            return (success, message);
        }

    }
}