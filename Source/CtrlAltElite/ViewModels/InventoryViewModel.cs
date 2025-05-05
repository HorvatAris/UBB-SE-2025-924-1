// <copyright file="InventoryViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using SteamStore.Models;
    using SteamStore.Services;
    using SteamStore.Services.Interfaces;

    public class InventoryViewModel : INotifyPropertyChanged
    {
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

            // Load users and initialize data.

            // this.LoadUsersAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the collection of inventory items.
        /// </summary>
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

        /// <summary>
        /// Gets the collection of available games.
        /// </summary>
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

        /// <summary>
        /// Gets the collection of available users.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the selected game filter.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the selected user.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the search text used to filter inventory items.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the currently selected inventory item.
        /// </summary>
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

        /// <summary>
        /// Initializes the Users and their inventories.
        /// </summary>
        /// <returns>Return a Task for asynchronous operations.</returns>
        public async Task InitializeAsync()
        {
            await this.LoadUsersAsync();
        }

        /// <summary>
        /// Asynchronously loads the inventory items and available games for the selected user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
                var availableGames = await this.inventoryService.GetAvailableGamesAsync(allItems);
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

        /// <summary>
        /// Sells an item asynchronously.
        /// </summary>
        /// <param name="selectedItem">The item who will be listed as for sale.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> SellItemAsync(Item selectedItem)
        {
            return await this.inventoryService.SellItemAsync(selectedItem);
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property that changed.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void UpdateAsyncVoid()
        {
            await this.UpdateInventoryItemsAsync();
        }

        /// <summary>
        /// Updates the filtered inventory items based on the current game filter and search text.
        /// </summary>
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

        /// <summary>
        /// Asynchronously loads the available users from the service.
        /// </summary>
        private async Task LoadUsersAsync()
        {
            try
            {
                var user = this.inventoryService.GetAllUsers();
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
    }
}