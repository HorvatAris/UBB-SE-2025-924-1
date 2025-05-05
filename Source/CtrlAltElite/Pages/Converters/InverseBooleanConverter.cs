// <copyright file="InverseBooleanConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Pages.Converters
{
    using System;
    using Microsoft.UI.Xaml.Data;

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool inputBooleanValue)
            {
                return !inputBooleanValue;
            }

            return value;
        }

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