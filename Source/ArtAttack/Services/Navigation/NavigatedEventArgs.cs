// <copyright file="NavigatedEventArgs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using System;
    using Microsoft.UI.Xaml.Navigation;

    /// <summary>
    /// Provides data for navigation events.
    /// </summary>
    public class NavigatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the parameter passed during navigation.
        /// </summary>
        public object? Parameter { get; set; }

        /// <summary>
        /// Gets or sets the navigation mode.
        /// </summary>
        public NavigationMode? NavigationMode { get; set; }

        /// <summary>
        /// Gets or sets the source page type.
        /// </summary>
        public Type? SourcePageType { get; set; }
    }
}