// <copyright file="IPageService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using System;

    /// <summary>
    /// Manages the mapping of ViewModels to their corresponding Page types.
    /// This service is used to resolve the correct Page type for a given ViewModel.
    /// </summary>
    public interface IPageService
    {
        /// <summary>
        /// Retrieves the Page type associated with a given ViewModel key.
        /// </summary>
        /// <param name="key">The unique key representing a ViewModel.</param>
        /// <returns>The Type of the associated Page.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the key is not found in the mapping.</exception>
        Type GetPageTypeForViewModel(string key);
    }
}