using CtrlAltElite.Models;
using SteamStore.Models;
using SteamStore.Services;
using SteamStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.ViewModels
{
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
            //this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            System.Diagnostics.Debug.WriteLine(inventoryS.ToString());
            inventoryService = inventoryS;
            inventoryItems = new ObservableCollection<Item>();
            availableGames = new ObservableCollection<Game>();
            availableUsers = new ObservableCollection<User>();

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
            get => inventoryItems;
            private set
            {
                if (inventoryItems != value)
                {
                    inventoryItems = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the collection of available games.
        /// </summary>
        public ObservableCollection<Game> AvailableGames
        {
            get => availableGames;
            private set
            {
                if (availableGames != value)
                {
                    availableGames = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the collection of available users.
        /// </summary>
        public ObservableCollection<User> AvailableUsers
        {
            get => availableUsers;
            private set
            {
                if (availableUsers != value)
                {
                    availableUsers = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected game filter.
        /// </summary>
        public Game SelectedGame
        {
            get => selectedGame;
            set
            {
                if (selectedGame != value)
                {
                    selectedGame = value;
                    OnPropertyChanged();

                    // Update the filtered inventory when the game filter changes.
                    UpdateInventoryItemsAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected user.
        /// </summary>
        public User SelectedUser
        {
            get => selectedUser;
            set
            {
                if (selectedUser != value && !isUpdating)
                {
                    selectedUser = value;
                    OnPropertyChanged();

                    // When a user is selected, load their inventory.
                    if (selectedUser != null)
                    {
                        _ = LoadInventoryItemsAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the search text used to filter inventory items.
        /// </summary>
        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    OnPropertyChanged();

                    // Update the filtered inventory when the search text changes.
                    this.updateAsyncVoid();
                }
            }
        }

        private async void updateAsyncVoid()
        {
            await UpdateInventoryItemsAsync();
        }

        /// <summary>
        /// Gets or sets the currently selected inventory item.
        /// </summary>
        public Item SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes the Users and their inventories.
        /// </summary>
        /// <returns>Return a Task for asynchronous operations.</returns>
        public async Task InitializeAsync()
        {
            await LoadUsersAsync();
        }

        /// <summary>
        /// Asynchronously loads the inventory items and available games for the selected user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task LoadInventoryItemsAsync()
        {
            if (SelectedUser == null)
            {
                return;
            }

            try
            {
                isUpdating = true;

                // Retrieve filtered inventory based on the current game filter and search text.
                var filteredItems = await inventoryService.GetUserFilteredInventoryAsync(
                    SelectedUser.UserId,
                    SelectedGame,
                    SearchText);

                // Update the inventory items collection.
                InventoryItems.Clear();
                foreach (var item in filteredItems)
                {
                    InventoryItems.Add(item);
                }

                // Retrieve all inventory items to rebuild the games filter.
                var allItems = await inventoryService.GetUserInventoryAsync(SelectedUser.UserId);
                var availableGames = inventoryService.GetAvailableGames(allItems);
                AvailableGames.Clear();
                foreach (var game in availableGames)
                {
                    AvailableGames.Add(game);
                }
            }
            catch (Exception loadingInventoryItemException)
            {
                // Log exception details as needed.
                System.Diagnostics.Debug.WriteLine($"Error loading inventory items: {loadingInventoryItemException.Message}");
                InventoryItems.Clear();
            }
            finally
            {
                isUpdating = false;
            }
        }

        /// <summary>
        /// Sells an item asynchronously.
        /// </summary>
        /// <param name="selectedItem">The item who will be listed as for sale.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> SellItemAsync(Item selectedItem)
        {
            return await inventoryService.SellItemAsync(selectedItem);
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property that changed.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates the filtered inventory items based on the current game filter and search text.
        /// </summary>
        private async Task UpdateInventoryItemsAsync()
        {
            if (SelectedUser == null)
            {
                return;
            }

            try
            {
                isUpdating = true;

                var filteredItems = await inventoryService.GetUserFilteredInventoryAsync(
                    SelectedUser.UserId,
                    SelectedGame,
                    SearchText);

                InventoryItems.Clear();
                foreach (var item in filteredItems)
                {
                    InventoryItems.Add(item);
                }
            }
            catch (Exception updatingInventoryItemsException)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating inventory items: {updatingInventoryItemsException.Message}");
                InventoryItems.Clear();
            }
            finally
            {
                isUpdating = false;
            }
        }

        /// <summary>
        /// Asynchronously loads the available users from the service.
        /// </summary>
        private async Task LoadUsersAsync()
        {
            try
            {
                var user = inventoryService.GetAllUsersAsync();
                AvailableUsers.Add(user);
                SelectedUser = user;
            }
            catch (Exception loadingUsersException)
            {
                // Log exception details as needed.
                System.Diagnostics.Debug.WriteLine($"Error loading users: {loadingUsersException.Message}");
                AvailableUsers.Clear();
            }
        }
    }
}