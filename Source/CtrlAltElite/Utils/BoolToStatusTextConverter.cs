// <copyright file="BoolToStatusTextConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml.Data;

    public class BoolToStatusTextConverter : IValueConverter
    {
        private const string ActiveText = "Active";
        private const string InactiveText = "Inactive";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isActive)
            {
                return isActive ? ActiveText : InactiveText;
            }

            return InactiveText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
