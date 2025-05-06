// <copyright file="BoolToOwnedStatusConverter.cs" company="PlaceholderCompany">
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

    public class BoolToOwnedStatusConverter : IValueConverter
    {
        private const string OwnedText = "Owned";
        private const string NotOwnedText = "Not Owned";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isOwned)
            {
                return isOwned ? OwnedText : NotOwnedText;
            }

            return NotOwnedText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
