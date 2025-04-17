// <copyright file="DiscountVisibilityConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Utils
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    public class DiscountVisibilityConverter : IValueConverter
    {
        public const int MinimumPositiveDiscountValue = 0;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal discount)
            {
                return discount > MinimumPositiveDiscountValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}