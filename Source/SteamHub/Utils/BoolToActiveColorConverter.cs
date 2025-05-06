// <copyright file="BoolToActiveColorConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Media;

    public class BoolToActiveColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isActive)
            {
                return isActive ? new SolidColorBrush(Microsoft.UI.Colors.Green) : new SolidColorBrush(Microsoft.UI.Colors.Gray);
            }

            return new SolidColorBrush(Microsoft.UI.Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
