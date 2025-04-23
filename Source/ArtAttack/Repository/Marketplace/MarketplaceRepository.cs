// <copyright file="MarketplaceRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Repository.Marketplace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Provides repository methods for interacting with marketplace items.
    /// Allows listing, unlisting, updating, and retrieving item listings associated with games.
    /// </summary>
    public class MarketplaceRepository : IMarketplaceRepository
    {
        /// <summary>
        /// Connector used to interact with the underlying database.
        /// </summary>
        private readonly DatabaseConnector databaseConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketplaceRepository"/> class.
        /// Sets up the database connector used for marketplace operations.
        /// </summary>
        public MarketplaceRepository()
        {
            this.databaseConnector = new DatabaseConnector();
        }

        /// <summary>
        /// Handles the purchase of an item by removing former ownership, adding the item to the new owner's inventory,
        /// and marking the item as unlisted.
        /// </summary>
        /// <param name="item"> Item to be purchased. </param>
        /// <param name="currentUser"> User that makes the item purchase. </param>
        /// <returns> True upon successful completion. </returns>
        /// <exception cref="InvalidOperationException"> Thrown in case of errors in transaction. </exception>
        public async Task<bool> BuyItemAsync(Item item, User currentUser)
        {
            try
            {
                // Start transaction
                await this.databaseConnector.OpenConnectionAsync();
                using (var transaction = this.databaseConnector.GetConnection().BeginTransaction())
                {
                    try
                    {
                        int currentOwnerID = await this.GetCurrentOwnerIDAsync(item, transaction);

                        await this.RemoveItemFromUserInventoryAsync(item, transaction);

                        await this.AddItemToUserInventoryAsync(item, currentUser, transaction);

                        await this.UpdateItemListedStatusAsync(item, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception buyItemTransactionException)
                    {
                        Debug.WriteLine($"Error in BuyItem transaction: {buyItemTransactionException.Message}");
                        Debug.WriteLine($"Stack trace: {buyItemTransactionException.StackTrace}");
                        transaction.Rollback();
                        throw new InvalidOperationException("Failed to complete purchase. Please try again.");
                    }
                }
            }
            finally
            {
                await this.databaseConnector.CloseConnectionAsync();
            }
        }

        /// <summary>
        /// Retrieves the ID of the current owner of an item.
        /// </summary>
        /// <param name="item"> Item from which the owner ID is retrieved. </param>
        /// <param name="transaction"> SQL Transaction. </param>
        /// <returns> The ID of the current owner of the item. </returns>
        public async Task<int> GetCurrentOwnerIDAsync(Item item, SqlTransaction transaction)
        {
            int currentOwnerId;
            using (var command = new SqlCommand(@"SELECT UserId FROM UserInventory WHERE ItemId = @ItemId", this.databaseConnector.GetConnection(), transaction))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                object? currentOwnerIDResult = await command.ExecuteScalarAsync();
                currentOwnerId = (int)currentOwnerIDResult;
            }

            return currentOwnerId;
        }

        /// <summary>
        /// Removes an item from a user's inventory.
        /// </summary>
        /// <param name="item"> Item to be removed from the user inventory. </param>
        /// <param name="transaction"> SQL Transaction. </param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task RemoveItemFromUserInventoryAsync(Item item, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(@"DELETE FROM UserInventory WHERE ItemId = @ItemId", this.databaseConnector.GetConnection(), transaction))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Adds an item to the user's inventory.
        /// </summary>
        /// <param name="item"> Item to be added to the user inventory. </param>
        /// <param name="user"> User that has the inventory to which the item will be added. </param>
        /// <param name="transaction"> SQL Transaction. </param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task AddItemToUserInventoryAsync(Item item, User user, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(@"INSERT INTO UserInventory (UserId, GameId, ItemId) VALUES (@UserId, @GameId, @ItemId)", this.databaseConnector.GetConnection(), transaction))
            {
                command.Parameters.AddWithValue("@UserId", user.UserId);
                command.Parameters.AddWithValue("@GameId", item.Game.GameId);
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Updates the listed item status to not listed.
        /// </summary>
        /// <param name="item"> Item for which the listed status will be updated. </param>
        /// <param name="transaction"> SQL Transaction. </param>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task UpdateItemListedStatusAsync(Item item, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(@"UPDATE Items SET IsListed = 0 WHERE ItemId = @ItemId", this.databaseConnector.GetConnection(), transaction))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Retrieves all listed items available for sale from the database.
        /// </summary>
        /// <returns>
        /// A list of Item objects that are currently listed for sale.
        /// </returns>
        public async Task<List<Item>> GetAllListedItemsAsync()
        {
            var items = new List<Item>();
            using (var command = new SqlCommand(
                @"SELECT i.ItemId, i.ItemName, i.Description, i.Price, i.IsListed,
                g.GameId, g.Title as GameTitle, g.Price as GamePrice, g.Genre, g.Description as GameDescription
                FROM Items i
                JOIN Games g ON i.CorrespondingGameId = g.GameId
                WHERE i.IsListed = 1",
                this.databaseConnector.GetConnection()))
            {
                try
                {
                    await this.databaseConnector.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var game = new Game(
                                reader.GetString(reader.GetOrdinal("GameTitle")),
                                (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                                reader.GetString(reader.GetOrdinal("Genre")),
                                reader.GetString(reader.GetOrdinal("GameDescription")));
                            game.SetGameId(reader.GetInt32(reader.GetOrdinal("GameId")));

                            var item = new Item(
                                reader.GetString(reader.GetOrdinal("ItemName")),
                                game,
                                (float)reader.GetDouble(reader.GetOrdinal("Price")),
                                reader.GetString(reader.GetOrdinal("Description")));
                            item.SetItemId(reader.GetInt32(reader.GetOrdinal("ItemId")));
                            item.SetIsListed(reader.GetBoolean(reader.GetOrdinal("IsListed")));

                            // Set image path based on game and item ID
                            string imagePath = this.databaseConnector.GetItemImagePath(item);
                            item.SetImagePath(imagePath);
                            System.Diagnostics.Debug.WriteLine($"Added listing item {item.ItemId} with image path: {imagePath}");

                            items.Add(item);
                        }
                    }
                }
                finally
                {
                    await this.databaseConnector.CloseConnectionAsync();
                }
            }

            return items;
        }

        /// <summary>
        /// Retrieves all currently listed items associated with a specific game.
        /// </summary>
        /// <param name="game">The Game entity for which to retrieve listed items.</param>
        /// <returns>
        /// A list of Item objects linked to the specified game and currently marked as listed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided <paramref name="game"/> is null.
        /// </exception>
        public async Task<List<Item>> GetListedItemsByGameAsync(Game game)
        {
            var items = new List<Item>();
            const string query = @"
                    SELECT i.ItemId, i.ItemName, i.Price, i.Description, i.IsListed,
                        g.GameId, g.Title as GameTitle, g.Genre, g.Description as GameDescription,
                        g.Price as GamePrice, g.Status as GameStatus
                    FROM Items i
                    JOIN UserInventory ui ON i.ItemId = ui.ItemId
                    JOIN Games g ON ui.GameId = g.GameId
                    WHERE g.GameId = @GameId AND i.IsListed = 1";

            try
            {
                using (var command = new SqlCommand(query, this.databaseConnector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@GameId", game.GameId);
                    await this.databaseConnector.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var gameObject = new Game(
                                reader.GetString(reader.GetOrdinal("GameTitle")),
                                (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                                reader.GetString(reader.GetOrdinal("Genre")),
                                reader.GetString(reader.GetOrdinal("GameDescription")));
                            gameObject.SetGameId(reader.GetInt32(reader.GetOrdinal("GameId")));
                            gameObject.SetStatus(reader.GetString(reader.GetOrdinal("GameStatus")));

                            var item = new Item(
                                reader.GetString(reader.GetOrdinal("ItemName")),
                                gameObject,
                                (float)reader.GetDouble(reader.GetOrdinal("Price")),
                                reader.GetString(reader.GetOrdinal("Description")));
                            item.SetItemId(reader.GetInt32(reader.GetOrdinal("ItemId")));
                            item.SetIsListed(reader.GetBoolean(reader.GetOrdinal("IsListed")));
                            items.Add(item);
                        }
                    }
                }
            }
            finally
            {
                await this.databaseConnector.CloseConnectionAsync();
            }

            return items;
        }

        /// <summary>
        /// Marks an existing item as listed for the specified game.
        /// </summary>
        /// <param name="game">The Game associated with the item to be listed.</param>
        /// <param name="item">The Item to be marked as listed and updated with a new price.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="game"/> or <paramref name="item"/> is null.
        /// </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task MakeItemListableAsync(Game game, Item item)
        {
            // AddListingAsync(Item item)
            using (var command = new SqlCommand(
                @"UPDATE Items 
                SET IsListed = 1, Price = @Price
                WHERE ItemId = @ItemId", this.databaseConnector.GetConnection()))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                command.Parameters.AddWithValue("@Price", item.Price);

                try
                {
                    await this.databaseConnector.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    await this.databaseConnector.CloseConnectionAsync();
                }
            }
        }

        /// <summary>
        /// Marks an item as not listable in the specified game by updating its listing status.
        /// </summary>
        /// <param name="game">The game in which the item exists.</param>
        /// <param name="item">The item to be marked as not listable.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="game"/> or <paramref name="item"/> is null.
        /// </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task MakeItemNotListableAsync(Game game, Item item)
        {
            // await this.databaseConnector.RemoveListingAsync(item);
            using (var command = new SqlCommand(
                    @"UPDATE Items 
                    SET IsListed = 0
                    WHERE ItemId = @ItemId", this.databaseConnector.GetConnection()))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);

                try
                {
                    await this.databaseConnector.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    await this.databaseConnector.CloseConnectionAsync();
                }
            }
        }

        /// <summary>
        /// Updates the price of an item in the specified game.
        /// </summary>
        /// <param name="game">The game in which the item exists.</param>
        /// <param name="item">The item whose price are being updated.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="game"/> or <paramref name="item"/> is null.
        /// </exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation. </returns>
        public async Task UpdateItemPriceAsync(Game game, Item item)
        {
            using (var command = new SqlCommand(
                @"UPDATE Items 
                SET Price = @Price
                WHERE ItemId = @ItemId", this.databaseConnector.GetConnection()))
            {
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                command.Parameters.AddWithValue("@Price", item.Price);

                try
                {
                    await this.databaseConnector.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    await this.databaseConnector.CloseConnectionAsync();
                }
            }
        }

        /// <inheritdoc/>
        public User? GetCurrentUser()
        {
            return this.databaseConnector.GetCurrentUser();
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await this.databaseConnector.GetAllUsersAsync();
        }
    }
}