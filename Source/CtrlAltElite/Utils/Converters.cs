// <copyright file="Converters.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Utils
{
    using System;
    using System.Globalization;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Media;
    using Windows.UI;

    public class BoolToActivateButtonTextConverter : IValueConverter
    {
        private const string ActivateText = "Activate";
        private const string DeactivateText = "Deactivate";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isActive)
            {
                return isActive ? DeactivateText : ActivateText;
            }

            return ActivateText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

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

    public class DateTimeToStringConverter : IValueConverter
    {
        private const string DateFormat = "MMM dd, yyyy HH:mm";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString(DateFormat); // Format: Mar 23, 2023 14:30
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

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

    public class UrlToPrettyTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string indexString && int.TryParse(indexString, out int index))
            {
                switch (index)
                {
                    case 0:
                        return "View Trailer";
                    case 1:
                        return "View Gameplay";
                }
            }

            return "View Media";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PriceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is decimal price ? $"${price:F2}" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}