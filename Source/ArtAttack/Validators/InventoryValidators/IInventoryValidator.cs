// <copyright file="IInventoryValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Validators.InventoryValidator.InventoryValidator
{
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Provides validation methods for inventory operations.
    /// </summary>
    public interface IInventoryValidator
    {
        /// <summary>
        /// Validates the specified game.
        /// </summary>
        /// <param name="game">The game to validate.</param>
        void ValidateGame(Game game);

        /// <summary>
        /// Validates the specified item.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        void ValidateItem(Item item);

        /// <summary>
        /// Validates the specified user.
        /// </summary>
        /// <param name="user">The user to validate.</param>
        void ValidateUser(User user);

        /// <summary>
        /// Validates all parameters for an inventory operation.
        /// </summary>
        /// <param name="game">The game to validate.</param>
        /// <param name="item">The item to validate.</param>
        /// <param name="user">The user to validate.</param>
        void ValidateInventoryOperation(Game game, Item item, User user);

        /// <summary>
        /// Validates whether an item is eligible to be sold.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <exception cref="InvalidOperationException">Thrown if the item is already listed.</exception>
        void ValidateSellableItem(Item item);
    }
}
