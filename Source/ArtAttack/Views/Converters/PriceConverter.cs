// <copyright file="PriceConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Views.Converters
{
    using System;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Converts a float price value to a formatted currency string.
    /// </summary>
    public partial class PriceConverter : IValueConverter
    {
        private const string CurrencySymbol = "$";
        private const string DefaultFormattedPrice = "$0.00";

        /// <summary>
        /// Converts a float price value to a formatted currency string using two decimal places.
        /// </summary>
        /// <param name="value">The value to convert. Expected to be of type <see cref="float"/>.</param>
        /// <param name="targetType">The target type of the binding.</param>
        /// <param name="parameter">An optional converter parameter.</param>
        /// <param name="language">The language information for the conversion.</param>
        /// <returns>
        /// A string formatted as currency, or "$0.00" if the input is not a valid float.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float productPrice)
            {
                return string.Format("{0}{1:N2}", CurrencySymbol, productPrice);
            }

            return DefaultFormattedPrice;
        }

        /// <summary>
        /// Not implemented. Converts the formatted price string back to a float.
        /// </summary>
        /// <param name="value">The value produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">An optional parameter.</param>
        /// <param name="language">The language information.</param>
        /// <returns>Always throws <see cref="NotImplementedException"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown in all cases because this method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}