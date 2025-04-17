// <copyright file="NotificationStrings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Constants
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class NotificationStrings
    {
        // Cart-related notifications
        public const string AddToCartSuccessTitle = "Success";
        public const string AddToCartErrorTitle = "Error";
        public const string AddToCartSuccessMessage = "{0} was added to your cart.";
        public const string AddToCartErrorMessage = "Failed to add {0} to your cart.";

        // Wishlist-related notifications
        public const string AddToWishlistSuccessTitle = "Success";
        public const string AddToWishlistErrorTitle = "Error";
        public const string AddToWishlistSuccessMessage = "{0} was added to your wishlist.";
        public const string AddToWishlistErrorMessage = "Failed to add {0} to your wishlist.";
        public const string AlreadyInWishlistErrorMessage = "Already in wishlist.";
    }
}
