// <copyright file="ExceptionMessages.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Constants
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class ExceptionMessages
    {
        public const string AllFieldsRequired = "All fields Are required!";
        public const string InvalidGameId = "GameId must be a valid integer!";
        public const string InvalidPrice = "Price must be a positive number.";
        public const string InvalidDiscount = "Discount must be between 0 and 100.";
        public const string NoTagsSelected = "Please select at least one tag for the game.";
        public const string GameIdInUse = "Game ID is already in use.";
        public const string FailedToCreateGame = "Failed to create game.";
        public const string FailedToUpdateGame = "Failed to update game.";
        public const string FailedToDeleteGame = "Failed to delete game.";
        public const string FailedToInsertGameTag = "Failed to insert game tag.";
        public const string FailedToDeleteGameTags = "Failed to delete game tags.";
        public const string FailedToRejectGame = "Failed to reject game.";
        public const string GameAlreadyOwned = "Failed to add {0} to your wishlist: Game already owned";
        public const string GameAlreadyInWishlist = "Failed to add {0} to your wishlist: Already in wishlist";
        public const string IdAlreadyInUse = "Game ID is already in use.";
    }
}
