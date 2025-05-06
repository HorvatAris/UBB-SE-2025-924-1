// <copyright file="CountToStringConverter.cs" company="PlaceholderCompany">
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

    public class CountToStringConverter : IValueConverter
    {
        private const string DefaultFormat = "{0}";
        private const int DefaultCount = 0;

        public string Format { get; set; } = DefaultFormat;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int count)
            {
                return string.Format(this.Format, count);
            }

            return string.Format(this.Format, DefaultCount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
