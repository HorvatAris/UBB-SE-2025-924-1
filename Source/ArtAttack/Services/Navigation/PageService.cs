// <copyright file="PageService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Manages the mapping of ViewModels to their corresponding Page types.
    /// This service is used to resolve the correct Page type for a given ViewModel.
    /// </summary>
    public class PageService : IPageService
    {
        private readonly Dictionary<string, Type> viewModelToPagesMap = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PageService"/> class with the given configuration options.
        /// </summary>
        /// <param name="options">Options containing ViewModel to Page mappings.</param>
        public PageService(IOptions<PageServiceOptions> options)
        {
            this.LoadPageMappingsFromOptions(options.Value);
        }

        /// <summary>
        /// Retrieves the Page type associated with a given ViewModel key.
        /// </summary>
        /// <param name="key">The unique key representing a ViewModel.</param>
        /// <returns>The Type of the associated Page.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the key is not found in the mapping.</exception>
        public Type GetPageTypeForViewModel(string key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key), "ViewModel key cannot be null.");
            }

            lock (this.viewModelToPagesMap)
            {
                if (!this.viewModelToPagesMap.TryGetValue(key, out var pageType))
                {
                    throw new ArgumentException($"Page not found: {key}. Did you forget to call LoadPageMappingsFromOptions?");
                }

                return pageType;
            }
        }

        /// <summary>
        /// Loads ViewModel-to-Page mappings from the provided options.
        /// </summary>
        /// <param name="options">The options containing the mappings to be loaded.</param>
        private void LoadPageMappingsFromOptions(PageServiceOptions options)
        {
            lock (this.viewModelToPagesMap)
            {
                foreach (var mapping in options.ViewModelToPagesMappings)
                {
                    this.viewModelToPagesMap.Add(mapping.Key, mapping.Value);
                }
            }
        }
    }
}