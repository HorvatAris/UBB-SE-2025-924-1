// <copyright file="INavigationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Represents the method that will handle navigation events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="NavigatedEventArgs"/> that contains the event data.</param>
    public delegate void NavigatedEventHandler(object sender, NavigatedEventArgs e);

    /// <summary>
    /// Provides an abstraction for navigation services.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Occurs when a navigation operation has completed.
        /// </summary>
        event NavigatedEventHandler Navigated;

        /// <summary>
        /// Gets a value indicating whether navigation backward is possible.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets or sets the frame used for navigation.
        /// </summary>
        Frame Frame { get; set; }

        /// <summary>
        /// Navigates to a specified page using a page key.
        /// </summary>
        /// <param name="pageKey">The key representing the target page.</param>
        /// <param name="parameter">Optional navigation parameter.</param>
        /// <param name="clearNavigation">If true, clears the navigation stack.</param>
        /// <returns>True if navigation succeeded; otherwise, false.</returns>
        bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false);

        /// <summary>
        /// Navigates to the previous page if possible.
        /// </summary>
        /// <returns>True if navigation backward succeeded; otherwise, false.</returns>
        bool GoBack();
    }
}