// <copyright file="NavigationHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    /// <summary>
    /// Provides attached properties to enable navigation from XAML elements.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Identifies the NavigateTo attached dependency property.
        /// </summary>
        public static readonly Microsoft.UI.Xaml.DependencyProperty NavigateToProperty =
            Microsoft.UI.Xaml.DependencyProperty.RegisterAttached(
                "NavigateTo",
                typeof(string),
                typeof(NavigationHelper),
                new Microsoft.UI.Xaml.PropertyMetadata(null));

        /// <summary>
        /// Gets the NavigateTo property value.
        /// </summary>
        /// <param name="uiTargetElement">The element to get the property from.</param>
        /// <returns>The navigation target page key.</returns>
        public static string GetNavigateTo(Microsoft.UI.Xaml.DependencyObject uiTargetElement)
        {
            return (string)uiTargetElement.GetValue(NavigateToProperty);
        }

        /// <summary>
        /// Sets the NavigateTo property value.
        /// </summary>
        /// <param name="uiTargetElement">The element to set the property on.</param>
        /// <param name="value">The navigation target page key.</param>
        public static void SetNavigateTo(Microsoft.UI.Xaml.DependencyObject uiTargetElement, string value)
        {
            uiTargetElement.SetValue(NavigateToProperty, value);
        }
    }
}