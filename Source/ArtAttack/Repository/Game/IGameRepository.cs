// <copyright file="IGameRepository.cs" company="PlaceholderCompany">
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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Provides data access functionality for <see cref="Game"/> entities.
    /// Communicates with the database through the <see cref="DatabaseConnector"/>.
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// Asynchronously retrieves all game records from the database.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of a list of games.
        /// </returns>
        Task<List<Game>> GetGamesAsync();

        /// <summary>
        /// Asynchronously retrieves a single game entity based on its unique identifier.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>
        /// A task with a result of the matching Game, or null if not found.
        /// </returns>
        Task<Game?> GetGameByIdAsync(int gameId);

        /// <summary>
        /// Asynchronously updates a game's information in the database.
        /// </summary>
        /// <param name="game">The game object to update.</param>
        /// <returns>True if update succeeded; otherwise, false.</returns>
        Task<bool> UpdateGameAsync(Game game);

        /// <summary>
        /// Asynchronously retrieves a list of all games from the database, ordered by title.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains a list of <see cref="Game"/> objects.</returns>
        Task<List<Game>> GetGamesFromDatabaseAsync();

        /// <summary>
        /// Asynchronously retrieves a specific game from the database based on its unique identifier.
        /// </summary>
        /// <param name="gameId">The ID of the game to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="Game"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Game?> GetGameByIdFromDatabaseAsync(int gameId);

        /// <summary>
        /// Asynchronously updates the details of an existing game in the database, excluding the status field.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> object containing updated information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> UpdateGameFromDatabaseAsync(Game game);
    }
}
