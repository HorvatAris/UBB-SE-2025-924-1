// <copyright file="INavigationAware.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    /// <summary>
    /// Provides methods that allow view models to respond to navigation events.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called when the view model has been navigated to.
        /// </summary>
        /// <param name="parameter">The navigation parameter passed during navigation.</param>
        void OnNavigatedTo(object parameter);

        /// <summary>
        /// Called when the view model is about to be navigated away from.
        /// </summary>
        void OnNavigatedFrom();
    }
}
