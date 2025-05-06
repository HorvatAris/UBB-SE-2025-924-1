// <copyright file="CurrencyConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages.Converters
{
    using System;
    using Microsoft.UI.Xaml.Data;

    public class CurrencyConverter : IValueConverter
    {
        private const string CurrencySymbol = "$";
        private const string DefaultFormattedCurrency = "$0.00";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float currencyValue)
            {
                return string.Format("{0}{1:F2}", CurrencySymbol, currencyValue);
            }

            return DefaultFormattedCurrency;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}