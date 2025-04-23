// <copyright file="IPageServiceOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Interface to store and manage ViewModel-to-Page mappings.
    /// Used to configure which ViewModel corresponds to which Page type.
    /// </summary>
    public interface IPageServiceOptions
    {
        /// <summary>
        /// Gets the read-only dictionary of ViewModel-to-Page mappings.
        /// </summary>
        IReadOnlyDictionary<string, Type> ViewModelToPagesMappings { get; }

        /// <summary>
        /// Configures a mapping between a ViewModel and a Page type.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <typeparam name="TView">The corresponding Page type.</typeparam>
        /// <returns>The current instance of <see cref="IPageServiceOptions"/> for method chaining.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the ViewModel is already mapped or if the Page type is already associated with another ViewModel.
        /// </exception>
        IPageServiceOptions Configure<TViewModel, TView>()
            where TViewModel : class
            where TView : Page;
    }
}
