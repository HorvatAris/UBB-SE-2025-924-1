// <copyright file="EmptyCollectionToVisibilityConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    public class EmptyCollectionToVisibilityConverter : IValueConverter
    {
        private const int EmptyCount = 0;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }

            if (value is int count)
            {
                return count == EmptyCount ? Visibility.Visible : Visibility.Collapsed;
            }

            // Try to handle ICollection types
            try
            {
                if (value is System.Collections.ICollection collection)
                {
                    return collection.Count == EmptyCount ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch
            {
                // Ignore errors :D
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
