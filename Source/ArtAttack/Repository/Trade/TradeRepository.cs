// <copyright file="TradeRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Repository.Trade
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;
    using Steampunks.Utils;

    /// <summary>
    /// Handles the database interactions.
    /// </summary>
    public class TradeRepository : ITradeRepository
    {
        private IDatabaseConnector? dataBaseConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeRepository"/> class.
        /// </summary>
        public TradeRepository()
        {
            this.dataBaseConnector = new DatabaseConnector();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeRepository"/> class.
        /// </summary>
        /// <param name="databaseConnector">the database connector.</param>
        public TradeRepository(IDatabaseConnector databaseConnector)
        {
            this.dataBaseConnector = databaseConnector;
        }

        /// <inheritdoc/>
        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            var trades = new List<ItemTrade>();
            const string tradesQuery = @"
                SELECT t.*, 
                       su.UserId as SourceUserId, su.Username as SourceUsername,
                       du.UserId as DestUserId, du.Username as DestUsername,
                       g.GameId, g.Title as GameTitle, g.Price as GamePrice, g.Genre, g.Description as GameDescription
                FROM ItemTrades t
                JOIN Users su ON t.SourceUserId = su.UserId
                JOIN Users du ON t.DestinationUserId = du.UserId
                JOIN Games g ON t.GameOfTradeId = g.GameId
                WHERE (t.SourceUserId = @UserId OR t.DestinationUserId = @UserId)
                AND t.TradeStatus = 'Pending'";

            const string itemsQuery = @"
                SELECT i.*, td.IsSourceUserItem,
                       g.GameId, g.Title as GameTitle, g.Price as GamePrice, g.Genre, g.Description as GameDescription
                FROM ItemTradeDetails td
                JOIN Items i ON td.ItemId = i.ItemId
                JOIN Games g ON i.CorrespondingGameId = g.GameId
                WHERE td.TradeId = @TradeId";

            using (var connection = this.dataBaseConnector.GetNewConnection())
            {
                await connection.OpenAsync();

                // First, get all trades
                using (var command = new SqlCommand(tradesQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var sourceUser = new User(
                                reader.GetString(reader.GetOrdinal("SourceUsername")));
                            sourceUser.SetUserId(reader.GetInt32(reader.GetOrdinal("SourceUserId")));

                            var destinationUser = new User(
                                reader.GetString(reader.GetOrdinal("DestUsername")));
                            destinationUser.SetUserId(reader.GetInt32(reader.GetOrdinal("DestUserId")));

                            var game = new Game(
                                reader.GetString(reader.GetOrdinal("GameTitle")),
                                (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                                reader.GetString(reader.GetOrdinal("Genre")),
                                reader.GetString(reader.GetOrdinal("GameDescription")));
                            game.SetGameId(reader.GetInt32(reader.GetOrdinal("GameId")));

                            var trade = new ItemTrade(
                                sourceUser,
                                destinationUser,
                                game,
                                reader.GetString(reader.GetOrdinal("TradeDescription")));

                            trade.SetTradeId(reader.GetInt32(reader.GetOrdinal("TradeId")));

                            // Set the trade status from the database
                            string tradeStatus = reader.GetString(reader.GetOrdinal("TradeStatus"));
                            if (tradeStatus == "Completed")
                            {
                                trade.MarkTradeAsCompleted();
                            }
                            else if (tradeStatus == "Declined")
                            {
                                trade.DeclineTradeRequest();
                            }

                            trades.Add(trade);
                        }
                    }
                }

                // Then, for each trade, get its items
                foreach (var trade in trades)
                {
                    using (var command = new SqlCommand(itemsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TradeId", trade.TradeId);
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

                                bool isSourceUserItem = reader.GetBoolean(reader.GetOrdinal("IsSourceUserItem"));
                                if (isSourceUserItem)
                                {
                                    trade.AddSourceUserItem(item);
                                }
                                else
                                {
                                    trade.AddDestinationUserItem(item);
                                }
                            }
                        }
                    }
                }
            }

            return trades;
        }

        /// <inheritdoc/>
        public async Task<List<ItemTrade>> GetTradeHistoryAsync(int userId)
        {
            var trades = new List<ItemTrade>();
            const string tradesQuery = @"
                SELECT t.*, 
                       su.UserId as SourceUserId, su.Username as SourceUsername,
                       du.UserId as DestUserId, du.Username as DestUsername,
                       g.GameId, g.Title as GameTitle, g.Price as GamePrice, g.Genre, g.Description as GameDescription
                FROM ItemTrades t
                JOIN Users su ON t.SourceUserId = su.UserId
                JOIN Users du ON t.DestinationUserId = du.UserId
                JOIN Games g ON t.GameOfTradeId = g.GameId
                WHERE (t.SourceUserId = @UserId OR t.DestinationUserId = @UserId)
                AND t.TradeStatus IN ('Completed', 'Declined')";

            const string itemsQuery = @"
                SELECT i.*, td.IsSourceUserItem,
                       g.GameId, g.Title as GameTitle, g.Price as GamePrice, g.Genre, g.Description as GameDescription
                FROM ItemTradeDetails td
                JOIN Items i ON td.ItemId = i.ItemId
                JOIN Games g ON i.CorrespondingGameId = g.GameId
                WHERE td.TradeId = @TradeId";

            using (var connection = this.dataBaseConnector.GetNewConnection())
            {
                await connection.OpenAsync();

                // First, get all trades
                using (var command = new SqlCommand(tradesQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var sourceUser = new User(
                                reader.GetString(reader.GetOrdinal("SourceUsername")));
                            sourceUser.SetUserId(reader.GetInt32(reader.GetOrdinal("SourceUserId")));

                            var destinationUser = new User(
                                reader.GetString(reader.GetOrdinal("DestUsername")));
                            destinationUser.SetUserId(reader.GetInt32(reader.GetOrdinal("DestUserId")));

                            var game = new Game(
                                reader.GetString(reader.GetOrdinal("GameTitle")),
                                (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                                reader.GetString(reader.GetOrdinal("Genre")),
                                reader.GetString(reader.GetOrdinal("GameDescription")));
                            game.SetGameId(reader.GetInt32(reader.GetOrdinal("GameId")));

                            var trade = new ItemTrade(
                                sourceUser,
                                destinationUser,
                                game,
                                reader.GetString(reader.GetOrdinal("TradeDescription")));
                            trade.SetTradeId(reader.GetInt32(reader.GetOrdinal("TradeId")));

                            // Set the trade status from the database
                            string tradeStatus = reader.GetString(reader.GetOrdinal("TradeStatus"));
                            if (tradeStatus == "Completed")
                            {
                                trade.MarkTradeAsCompleted();
                            }
                            else if (tradeStatus == "Declined")
                            {
                                trade.DeclineTradeRequest();
                            }

                            trades.Add(trade);
                        }
                    }
                }

                // Then, for each trade, get its items
                foreach (var trade in trades)
                {
                    using (var command = new SqlCommand(itemsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TradeId", trade.TradeId);
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

                                bool isSourceUserItem = reader.GetBoolean(reader.GetOrdinal("IsSourceUserItem"));
                                if (isSourceUserItem)
                                {
                                    trade.AddSourceUserItem(item);
                                }
                                else
                                {
                                    trade.AddDestinationUserItem(item);
                                }
                            }
                        }
                    }
                }
            }

            return trades;
        }

        /// <inheritdoc/>
        public async Task AddItemTradeAsync(ItemTrade trade)
        {
            const string insertTrade = @"
                INSERT INTO ItemTrades (SourceUserId, DestinationUserId, GameOfTradeId, TradeDate, TradeDescription, TradeStatus, AcceptedBySourceUser, AcceptedByDestinationUser)
                OUTPUT INSERTED.TradeId
                VALUES (@SourceUserId, @DestinationUserId, @GameId, @TradeDate, @TradeDescription, @TradeStatus, 1, 0)";

            const string insertTradeDetails = @"
                INSERT INTO ItemTradeDetails (TradeId, ItemId, IsSourceUserItem)
                VALUES (@TradeId, @ItemId, @IsSourceUserItem)";

            using (var connection = this.dataBaseConnector.GetNewConnection())
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert trade (note that AcceptedBySourceUser is set to 1)
                        int tradeId;
                        using (var command = new SqlCommand(insertTrade, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SourceUserId", trade.SourceUser.UserId);
                            command.Parameters.AddWithValue("@DestinationUserId", trade.DestinationUser.UserId);
                            command.Parameters.AddWithValue("@GameId", trade.GameOfTrade.GameId);
                            command.Parameters.AddWithValue("@TradeDate", trade.TradeDate);
                            command.Parameters.AddWithValue("@TradeDescription", trade.TradeDescription);
                            command.Parameters.AddWithValue("@TradeStatus", "Pending");

                            var insertedID = await command.ExecuteScalarAsync();
                            if (insertedID == null)
                            {
                                throw new Exception("Error inserting a new trade");
                            }

                            tradeId = (int)insertedID;
                        }

                        // Insert source user items
                        foreach (var item in trade.SourceUserItems)
                        {
                            using (var command = new SqlCommand(insertTradeDetails, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@TradeId", tradeId);
                                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                                command.Parameters.AddWithValue("@IsSourceUserItem", true);
                                command.ExecuteNonQuery();
                            }
                        }

                        // Insert destination user items
                        foreach (var item in trade.DestinationUserItems)
                        {
                            using (var command = new SqlCommand(insertTradeDetails, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@TradeId", tradeId);
                                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                                command.Parameters.AddWithValue("@IsSourceUserItem", false);
                                command.ExecuteNonQuery();
                            }
                        }

                        await transaction.CommitAsync();
                        trade.SetTradeId(tradeId);
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task UpdateItemTradeAsync(ItemTrade trade)
        {
            const string updateTradeQuery = @"
                UPDATE ItemTrades 
                SET TradeStatus = @TradeStatus,
                    AcceptedByDestinationUser = @AcceptedByDestinationUser
                WHERE TradeId = @TradeId";

            const string getTradeItemsQuery = @"
                SELECT ItemId, IsSourceUserItem
                FROM ItemTradeDetails
                WHERE TradeId = @TradeId";

            using (var connection = this.dataBaseConnector.GetNewConnection())
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update the trade status and destination user acceptance
                        using (var command = new SqlCommand(updateTradeQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@TradeId", trade.TradeId);

                            // If destination user accepts, mark trade as completed since source user already accepted
                            command.Parameters.AddWithValue("@TradeStatus", trade.AcceptedByDestinationUser ? "Completed" : trade.TradeStatus);
                            command.Parameters.AddWithValue("@AcceptedByDestinationUser", trade.AcceptedByDestinationUser);

                            await command.ExecuteNonQueryAsync();
                        }

                        // If the destination user accepted, transfer the items
                        if (trade.AcceptedByDestinationUser)
                        {
                            // Get all items involved in the trade
                            var itemsToTransfer = new List<(int ItemId, bool IsSourceUserItem)>();
                            using (var command = new SqlCommand(getTradeItemsQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@TradeId", trade.TradeId);
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        itemsToTransfer.Add((
                                            reader.GetInt32(reader.GetOrdinal("ItemId")),
                                            reader.GetBoolean(reader.GetOrdinal("IsSourceUserItem"))));
                                    }
                                }
                            }

                            // Transfer each item
                            foreach (var (itemId, isSourceUserItem) in itemsToTransfer)
                            {
                                int fromUserId = isSourceUserItem ? trade.SourceUser.UserId : trade.DestinationUser.UserId;
                                int toUserId = isSourceUserItem ? trade.DestinationUser.UserId : trade.SourceUser.UserId;

                                const string transferQuery = @"
                                    UPDATE UserInventory
                                    SET UserId = @ToUserId
                                    WHERE ItemId = @ItemId AND UserId = @FromUserId";

                                using (var command = new SqlCommand(transferQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@ItemId", itemId);
                                    command.Parameters.AddWithValue("@FromUserId", fromUserId);
                                    command.Parameters.AddWithValue("@ToUserId", toUserId);
                                    int rowsAffected = await command.ExecuteNonQueryAsync();
                                    if (rowsAffected == 0)
                                    {
                                        throw new Exception($"Failed to transfer item {itemId} from user {fromUserId} to user {toUserId}");
                                    }

                                    System.Diagnostics.Debug.WriteLine($"Successfully transferred item {itemId} from user {fromUserId} to user {toUserId}");
                                }
                            }
                        }

                        await transaction.CommitAsync();
                        System.Diagnostics.Debug.WriteLine($"Trade {trade.TradeId} updated successfully. Status: {trade.TradeStatus}");
                    }
                    catch (Exception tradeUpdatingException)
                    {
                        try
                        {
                            await transaction.RollbackAsync();
                        }
                        catch (Exception rollbackException)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error rolling back transaction: {rollbackException.Message}");
                        }

                        System.Diagnostics.Debug.WriteLine($"Error updating trade: {tradeUpdatingException.Message}");
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task TransferItemAsync(int itemId, int fromUserId, int toUserId)
        {
            const string query = @"
                UPDATE UserInventory
                SET UserId = @ToUserId
                WHERE ItemId = @ItemId AND UserId = @FromUserId";

            using (var connection = this.dataBaseConnector.GetNewConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ItemId", itemId);
                    command.Parameters.AddWithValue("@FromUserId", fromUserId);
                    command.Parameters.AddWithValue("@ToUserId", toUserId);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Failed to transfer item {itemId} from user {fromUserId} to user {toUserId}");
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task<User?> GetCurrentUserAsync()
        {
            using (var connection = this.dataBaseConnector.GetNewConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT TOP 1 UserId, Username FROM Users", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var user = new User(reader.GetString(reader.GetOrdinal("Username")));
                            user.SetUserId(reader.GetInt32(reader.GetOrdinal("UserId")));
                            return user;
                        }

                        return null;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            var items = new List<Item>();
            const string query = @"
                SELECT 
                    i.ItemId,
                    i.ItemName,
                    i.Price,
                    i.Description,
                    i.IsListed,
                    g.GameId,
                    g.Title as GameTitle,
                    g.Genre,
                    g.Description as GameDescription,
                    g.Price as GamePrice,
                    g.Status as GameStatus
                FROM Items i
                JOIN Games g ON i.CorrespondingGameId = g.GameId
                JOIN UserInventory ui ON i.ItemId = ui.ItemId AND g.GameId = ui.GameId
                WHERE ui.UserId = @UserId
                ORDER BY g.Title, i.Price";

            try
            {
                using (var connection = this.dataBaseConnector.GetNewConnection())
                {
                    await connection.OpenAsync();
                    System.Diagnostics.Debug.WriteLine($"Executing GetUserInventory query for userId: {userId}");
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        System.Diagnostics.Debug.WriteLine("Connection opened successfully");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            System.Diagnostics.Debug.WriteLine("Query executed successfully");
                            while (await reader.ReadAsync())
                            {
                                System.Diagnostics.Debug.WriteLine($"Found item: {reader.GetString(reader.GetOrdinal("ItemName"))}");
                                var game = new Game(
                                    reader.GetString(reader.GetOrdinal("GameTitle")),
                                    (float)reader.GetDouble(reader.GetOrdinal("GamePrice")),
                                    reader.GetString(reader.GetOrdinal("Genre")),
                                    reader.GetString(reader.GetOrdinal("GameDescription")));
                                game.SetGameId(reader.GetInt32(reader.GetOrdinal("GameId")));
                                game.SetStatus(reader.GetString(reader.GetOrdinal("GameStatus")));

                                var item = new Item(
                                    reader.GetString(reader.GetOrdinal("ItemName")),
                                    game,
                                    (float)reader.GetDouble(reader.GetOrdinal("Price")),
                                    reader.GetString(reader.GetOrdinal("Description")));
                                item.SetItemId(reader.GetInt32(reader.GetOrdinal("ItemId")));
                                item.SetIsListed(reader.GetBoolean(reader.GetOrdinal("IsListed")));

                                // Set image path based on game and item name
                                string imagePath = this.GetItemImagePath(item);
                                item.SetImagePath(imagePath);

                                items.Add(item);
                            }

                            System.Diagnostics.Debug.WriteLine($"Total items found: {items.Count}");
                        }
                    }
                }
            }
            catch (Exception getUserInventoryException)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetUserInventory: {getUserInventoryException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {getUserInventoryException.StackTrace}");
                if (getUserInventoryException.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {getUserInventoryException.InnerException.Message}");
                }

                throw;
            }

            return items;
        }

        private string GetItemImagePath(Item item)
        {
            try
            {
                // Get the game folder name based on the game title
                string gameFolder = item.Game.Title.ToLower() switch
                {
                    "counter-strike 2" => "cs2",
                    "dota 2" => "dota2",
                    "team fortress 2" => "tf2",
                    _ => item.Game.Title.ToLower().Replace(" ", string.Empty).Replace(":", string.Empty)
                };

                // Return a path to the image based on the ItemId
                var path = $"ms-appx:///Assets/img/games/{gameFolder}/{item.ItemId}.png";
                System.Diagnostics.Debug.WriteLine($"Generated image path for item {item.ItemId} ({item.ItemName}) from {item.Game.Title}: {path}");
                return path;
            }
            catch (Exception getItemImagePathException)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetItemImagePath: {getItemImagePathException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {getItemImagePathException.StackTrace}");
                return "ms-appx:///Assets/img/games/default-item.png";
            }
        }
    }
}
