// <copyright file="InverseBooleanConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages.Converters
{
    using System;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Converts a boolean value to its inverse.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to its opposite.
        /// </summary>
        /// <param name="value">The value to convert. Expected to be of type <see cref="bool"/>.</param>
        /// <param name="targetType">The target type of the binding.</param>
        /// <param name="parameter">An optional converter parameter.</param>
        /// <param name="language">The language information for the conversion.</param>
        /// <returns>
        /// The logical negation of the boolean value if valid; otherwise, returns the original value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool inputBooleanValue)
            {
                return !inputBooleanValue;
            }

            return value;
        }

        /// <summary>
        /// Converts the value back to its original boolean state by inverting it again.
        /// </summary>
        /// <param name="value">The value produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">An optional converter parameter.</param>
        /// <param name="language">The language information.</param>
        /// <returns>
        /// The logical negation of the boolean value if valid; otherwise, returns the original value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool inputBooleanValue)
            {
                return !inputBooleanValue;
            }

            return value;
        }
    }
}