// <copyright file="IGameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Interface that defines operations for accessing and managing Game data.
// </summary>
// Added by Team ArtAttack, 2025

namespace Steampunks.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Defines operations for retrieving and updating game data.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Asynchronously retrieves all games from the database.
        /// </summary>
        /// <returns>A list of all games.</returns>
        Task<List<Game>> GetAllGamesAsync();

        /// <summary>
        /// Asynchronously retrieves a game by its ID.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <returns>The game with the specified ID.</returns>
        Task<Game?> GetGameByIdAsync(int gameId);

        /// <summary>
        /// Asynchronously updates a game's information in the database.
        /// </summary>
        /// <param name="game">The game object to update.</param>
        /// <returns>True if update succeeded; otherwise, false.</returns>
        Task<bool> UpdateGameAsync(Game game);
    }
}
