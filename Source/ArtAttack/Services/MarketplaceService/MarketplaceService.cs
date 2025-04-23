// <copyright file="MarketplaceService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services.MarketplaceService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;
    using Steampunks.Repository.Marketplace;

    /// <summary>
    /// Service that manages marketplace operations.
    /// </summary>
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IMarketplaceRepository marketplaceRepository;
        private User currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketplaceService"/> class.
        /// </summary>
        /// <param name="marketplaceRepository"> Marketplace repository. </param>
        /// <exception cref="ArgumentNullException"> Thrown if repository is null. </exception>
        public MarketplaceService(IMarketplaceRepository marketplaceRepository)
        {
            if (marketplaceRepository != null)
            {
                this.marketplaceRepository = marketplaceRepository;
            }
            else
            {
                throw new ArgumentNullException(nameof(marketplaceRepository));
            }

            this.currentUser = this.marketplaceRepository.GetCurrentUser();
        }

        /// <summary>
        /// Returns the current user.
        /// </summary>
        /// <returns> The current user. </returns>
        public User GetCurrentUser()
        {
            return this.currentUser;
        }

        /// <summary>
        /// Sets the current user to another.
        /// </summary>
        /// <param name="user"> User to be set. </param>
        /// <exception cref="ArgumentNullException"> Thrown if user is null. </exception>
        public void SetCurrentUser(User user)
        {
            if (user != null)
            {
                this.currentUser = user;
            }
            else
            {
                throw new ArgumentNullException(nameof(user));
            }
        }

        /// <summary>
        /// Retrieve all users from the database.
        /// </summary>
        /// <returns> A list of all Users. </returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await this.marketplaceRepository.GetAllUsersAsync();
        }

        /// <summary>
        /// Retrieves all the listings in the marketplace.
        /// </summary>
        /// <returns> A list of Items (marketplace listings). </returns>
        public async Task<List<Item>> GetAllListingsAsync()
        {
            return await this.marketplaceRepository.GetAllListedItemsAsync();
        }

        /// <summary>
        /// Retrieves listings from a certain game.
        /// </summary>
        /// <param name="game"> Game for which the listing is retrieved. </param>
        /// <returns> A list of Items (marketplace listings). </returns>
        /// <exception cref="ArgumentNullException"> Thrown if game is null. </exception>
        public async Task<List<Item>> GetListingsByGameAsync(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            return await this.marketplaceRepository.GetListedItemsByGameAsync(game);
        }

        /// <summary>
        /// Adds listing (an item from a game).
        /// </summary>
        /// <param name="game"> Game from which the item to be listed is. </param>
        /// <param name="item"> Item to be added as listing. </param>
        /// <exception cref="ArgumentNullException"> Thrown if either the game or item is null. </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task AddListingAsync(Game game, Item item)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.marketplaceRepository.MakeItemListableAsync(game, item);
        }

        /// <summary>
        /// Remove listing (an item from a game).
        /// </summary>
        /// <param name="game"> Game from which the item to be listed is. </param>
        /// <param name="item"> Item listing to be removed. </param>
        /// <exception cref="ArgumentNullException"> Thrown if either the game or item is null. </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task RemoveListingAsync(Game game, Item item)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.marketplaceRepository.MakeItemListableAsync(game, item);
        }

        /// <summary>
        /// Update listing (an item from a game).
        /// </summary>
        /// <param name="game"> Game from which the item to be listed is. </param>
        /// <param name="item"> Item listing to be updated. </param>
        /// <exception cref="ArgumentNullException"> Thrown if either the game or item is null. </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task UpdateListingAsync(Game game, Item item)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.marketplaceRepository.UpdateItemPriceAsync(game, item);
        }

        /// <summary>
        /// Handles the purchase of an item by removing former ownership, adding the item to the new owner's inventory,
        /// and marking the item as unlisted.
        /// </summary>
        /// <param name="item"> Item to be purchased. </param>
        /// <returns> True upon successful purchase. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when the item is null. </exception>
        /// <exception cref="InvalidOperationException"> Thrown when either the item is not listed or when the current user is null. </exception>
        public async Task<bool> BuyItemAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsListed)
            {
                throw new InvalidOperationException("Item is not listed for sale");
            }

            return await this.marketplaceRepository.BuyItemAsync(item, this.currentUser);
        }
    }
}
