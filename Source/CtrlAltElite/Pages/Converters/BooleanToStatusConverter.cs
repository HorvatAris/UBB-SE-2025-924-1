// <copyright file="BooleanToStatusConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages.Converters
{
    using System;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Converts a boolean value to a user-friendly status string: "Listed", "Not Listed", or "Unknown".
    /// </summary>
    public class BooleanToStatusConverter : IValueConverter
    {
        private const string StatusWhenListed = "Listed";
        private const string StatusWhenNotListed = "Not Listed";
        private const string StatusWhenUnknown = "Unknown";

        // </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isItemListed)
            {
                return isItemListed ? StatusWhenListed : StatusWhenNotListed;
            }

            return StatusWhenUnknown;
        }

        /// <summary>
        /// Not implemented. Converts a status string back to a boolean.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Optional parameter used in the converter.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>Always throws <see cref="NotImplementedException"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown in all cases because this method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}