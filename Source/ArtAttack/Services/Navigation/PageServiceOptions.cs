// <copyright file="PageServiceOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Stores and manages ViewModel-to-Page mappings.
    /// Used to configure which ViewModel corresponds to which Page type.
    /// </summary>
    public class PageServiceOptions : IPageServiceOptions
    {
        private readonly Dictionary<string, Type> viewModelToPagesMap = new Dictionary<string, Type>();

        /// <summary>
        /// Gets the read-only dictionary of ViewModel-to-Page mappings.
        /// </summary>
        public IReadOnlyDictionary<string, Type> ViewModelToPagesMappings => this.viewModelToPagesMap;

        /// <summary>
        /// Configures a mapping between a ViewModel and a Page type.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <typeparam name="TView">The corresponding Page type.</typeparam>
        /// <returns>The current instance of PageServiceOptions for method chaining.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the ViewModel is already mapped or if the Page type is already associated with another ViewModel.
        /// </exception>
        public IPageServiceOptions Configure<TViewModel, TView>()
            where TViewModel : class
            where TView : Page
        {
            var viewModelKey = typeof(TViewModel).FullName;
            var pageType = typeof(TView);

            if (viewModelKey is null)
            {
                throw new ArgumentNullException(nameof(viewModelKey));
            }

            if (this.viewModelToPagesMap.ContainsKey(viewModelKey))
            {
                throw new ArgumentException($"The viewModelKey {viewModelKey} is already configured in {nameof(PageServiceOptions)}");
            }

            if (this.viewModelToPagesMap.Any(mapping => mapping.Value == pageType))
            {
                throw new ArgumentException($"This pageType is already configured with viewModelKey {this.viewModelToPagesMap.First(mapping => mapping.Value == pageType).Key}");
            }

            this.viewModelToPagesMap.Add(viewModelKey, pageType);
            return this;
        }
    }
}
