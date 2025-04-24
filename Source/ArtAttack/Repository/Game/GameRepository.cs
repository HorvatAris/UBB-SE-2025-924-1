// <copyright file="GameRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Service class for accessing and managing Game data.
// </summary>
// Refactored by Team ArtAttack, 2025
namespace Steampunks.Repository.GameRepo
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Provides data access functionality for <see cref="Game"/> entities.
    /// Communicates with the database through the <see cref="DatabaseConnector"/>.
    /// </summary>
    public class GameRepository : IGameRepository
    {
        private const string GetAllGamesQuery = @"
    SELECT 
        GameId,
        GameTitle,
        Price,
        Genre,
        GameDescription,
        Status
    FROM Games
    ORDER BY GameTitle";

        private const string GetGameByIdQuery = @"
    SELECT 
        GameId,
        GameTitle,
        Price,
        Genre,
        GameDescription,
        Status
    FROM Games
    WHERE GameId = @GameId";

        private const string UpdateGameQuery = @"
    UPDATE Games
    SET GameTitle = @GameTitle,
        Price = @Price,
        Genre = @Genre,
        GameDescription = @GameDescription
    WHERE GameId = @GameId";

        private const string ColumnGameId = "GameId";
        private const string ColumnTitle = "GameTitle";
        private const string ColumnPrice = "Price";
        private const string ColumnGenre = "Genre";
        private const string ColumnDescription = "GameDescription";
        private const string ColumnStatus = "Status";

        /// <summary>
        /// Responsible for executing database operations related to games.
        /// </summary>
        private readonly IDatabaseConnector databaseConnector;

        private SqlConnection? connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRepository"/> class
        /// and sets up a connection to the database.
        /// </summary>
        /// <param name="databaseConnector">The <see cref="DatabaseConnector"/> used to connect to the database.</param>
        public GameRepository(IDatabaseConnector databaseConnector)
        {
            this.databaseConnector = databaseConnector ?? throw new ArgumentNullException(nameof(databaseConnector));
        }

        public GameRepository()
        {
            this.databaseConnector = new DatabaseConnector();
        }

        /// <summary>
        /// Retrieves all game records from the database.
        /// </summary>
        /// <returns>
        /// A list of Game entities representing all games in the database.
        /// </returns>
        public async Task<List<Game>> GetGamesAsync()
        {
            return await this.GetGamesFromDatabaseAsync();
        }

        /// <summary>
        /// Retrieves a single game entity based on its unique identifier.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to retrieve.</param>
        /// <returns>
        /// A Game object if a game with the specified <paramref name="gameId"/> exists; otherwise, null.
        /// </returns>
        public async Task<Game?> GetGameByIdAsync(int gameId)
        {
            return await this.GetGameByIdFromDatabaseAsync(gameId);
        }

        /// <summary>
        /// Asynchronously updates a game's information in the database.
        /// </summary>
        /// <param name="game">The game object to update.</param>
        /// <returns>True if update succeeded; otherwise, false.</returns>
        public async Task<bool> UpdateGameAsync(Game game)
        {
            return await this.UpdateGameFromDatabaseAsync(game);
        }

        /// <summary>
        /// Asynchronously retrieves a list of all games from the database, ordered by title.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of <see cref="Game"/> objects.</returns>
        public async Task<List<Game>> GetGamesFromDatabaseAsync()
        {
            var games = new List<Game>();

            try
            {
                using (var command = new SqlCommand(GetAllGamesQuery, this.databaseConnector.GetConnection()))
                {
                    await this.databaseConnector.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var game = new Game(
                                reader.GetString(reader.GetOrdinal(ColumnTitle)),
                                (float)reader.GetDouble(reader.GetOrdinal(ColumnPrice)),
                                reader.GetString(reader.GetOrdinal(ColumnGenre)),
                                reader.GetString(reader.GetOrdinal(ColumnDescription)));
                            game.SetGameId(reader.GetInt32(reader.GetOrdinal(ColumnGameId)));
                            game.SetStatus(reader.GetString(reader.GetOrdinal(ColumnStatus)));
                            games.Add(game);
                        }
                    }
                }
            }
            finally
            {
                this.databaseConnector.CloseConnection();
            }

            return games;
        }

        /// <summary>
        /// Asynchronously retrieves a specific game from the database based on its unique identifier.
        /// </summary>
        /// <param name="gameId">The ID of the game to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="Game"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<Game?> GetGameByIdFromDatabaseAsync(int gameId)
        {
            try
            {
                using (var command = new SqlCommand(GetGameByIdQuery, this.databaseConnector.GetConnection()))
                {
                    command.Parameters.AddWithValue(ColumnGameId, gameId);
                    await this.databaseConnector.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var game = new Game(
                                reader.GetString(reader.GetOrdinal(ColumnTitle)),
                                (float)reader.GetDouble(reader.GetOrdinal(ColumnPrice)),
                                reader.GetString(reader.GetOrdinal(ColumnGenre)),
                                reader.GetString(reader.GetOrdinal(ColumnDescription)));
                            game.SetGameId(reader.GetInt32(reader.GetOrdinal(ColumnGameId)));
                            game.SetStatus(reader.GetString(reader.GetOrdinal(ColumnStatus)));
                            return game;
                        }
                    }
                }
            }
            finally
            {
                this.databaseConnector.CloseConnection();
            }

            return null;
        }

        /// <summary>
        /// Asynchronously updates the details of an existing game in the database, excluding the status field.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> object containing updated information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> UpdateGameFromDatabaseAsync(Game game)
        {
            try
            {
                using (var command = new SqlCommand(UpdateGameQuery, this.databaseConnector.GetConnection()))
                {
                    command.Parameters.AddWithValue(ColumnGameId, game.GameId);
                    command.Parameters.AddWithValue(ColumnTitle, game.GameTitle);
                    command.Parameters.AddWithValue(ColumnPrice, game.Price);
                    command.Parameters.AddWithValue(ColumnGenre, game.Genre);
                    command.Parameters.AddWithValue(ColumnDescription, game.GameDescription);

                    await this.databaseConnector.OpenConnectionAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            finally
            {
                this.databaseConnector.CloseConnection();
            }
        }
    }
}