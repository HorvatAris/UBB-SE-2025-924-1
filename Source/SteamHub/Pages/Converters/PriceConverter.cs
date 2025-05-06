// <copyright file="PriceConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages.Converters
{
    using System;
    using Microsoft.UI.Xaml.Data;

    public partial class PriceConverter : IValueConverter
    {
        private const string CurrencySymbol = "$";
        private const string DefaultFormattedPrice = "$0.00";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float productPrice)
            {
                return string.Format("{0}{1:N2}", CurrencySymbol, productPrice);
            }

            return DefaultFormattedPrice;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}