// <copyright file="INavigationViewService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Defines the interface for a service that manages the NavigationView control and navigation logic.
    /// </summary>
    public interface INavigationViewService
    {
        /// <summary>
        /// Gets the navigation service used to perform navigation operations.
        /// </summary>
        INavigationService NavigationService { get; }

        /// <summary>
        /// Gets or sets the NavigationView instance.
        /// </summary>
        NavigationView? NavigationView { get; set; }

        /// <summary>
        /// Initializes the NavigationViewService with a NavigationView control.
        /// </summary>
        /// <param name="navigationView">The NavigationView control to initialize.</param>
        void Initialize(NavigationView navigationView);

        /// <summary>
        /// Unregisters all NavigationView-related events.
        /// </summary>
        void UnregisterEvents();
    }
}