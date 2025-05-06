// <copyright file="BooleanToStatusConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages.Converters
{
    using System;
    using Microsoft.UI.Xaml.Data;

    public class BooleanToStatusConverter : IValueConverter
    {
        private const string StatusWhenListed = "Listed";
        private const string StatusWhenNotListed = "Not Listed";
        private const string StatusWhenUnknown = "Unknown";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isItemListed)
            {
                return isItemListed ? StatusWhenListed : StatusWhenNotListed;
            }

            return StatusWhenUnknown;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}