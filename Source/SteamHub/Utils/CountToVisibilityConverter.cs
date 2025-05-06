// <copyright file="CountToVisibilityConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Utils
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    public class CountToVisibilityConverter : IValueConverter
    {
        private const int ZeroCount = 0;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int count)
            {
                return count == ZeroCount ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}