// <copyright file="InventoryValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.User;

    public class InventoryValidator
    {
        public void ValidateGame(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game), "Game cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(game.GameTitle))
            {
                throw new ArgumentException("Game title cannot be null or empty.", nameof(game));
            }

            if (game.Price < 0)
            {
                throw new ArgumentException("Game price cannot be negative.", nameof(game));
            }

            if (game.GameId <= 0)
            {
                throw new ArgumentException("Game must have a valid positive ID.", nameof(game));
            }
        }

        public void ValidateItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(item.ItemName))
            {
                throw new ArgumentException("Item name cannot be null or empty.", nameof(item));
            }

            // Expanded validation: ensure the item has a valid positive ID.
            if (item.ItemId <= 0)
            {
                throw new ArgumentException("Item must have a valid positive ID.", nameof(item));
            }
        }

        public void ValidateUser(IUserDetails user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new ArgumentException("User username cannot be null or empty.", nameof(user));
            }

            // Expanded validation: ensure the user has a valid positive ID.
            if (user.UserId <= 0)
            {
                throw new ArgumentException("User must have a valid positive ID.", nameof(user));
            }
        }

        public void ValidateInventoryOperation(Game game, Item item, IUserDetails user)
        {
            // Reuse the individual validations.
            this.ValidateGame(game);
            this.ValidateItem(item);
            this.ValidateUser(user);
        }

        public void ValidateSellableItem(Item item)
        {
            // First, validate the basic properties of the item.
            this.ValidateItem(item);

            // Expanded validation: ensure the item is not already listed for sale.
            if (item.IsListed)
            {
                throw new InvalidOperationException("Item is already listed and cannot be sold.");
            }
        }
    }
}