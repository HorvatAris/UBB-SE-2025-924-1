// <copyright file="NavigationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using System;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;

    /// <summary>
    /// Provides a service for navigating between pages and managing navigation history.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IPageService? pageService;
        private object? lastParameterUsed;
        private Frame? frame;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="pageService">The service used to resolve page types from page keys.</param>
        public NavigationService(IPageService pageService)
        {
            this.pageService = pageService;
        }

        /// <inheritdoc/>
        public event NavigatedEventHandler? Navigated;

        /// <inheritdoc/>
        public Frame Frame
        {
            get
            {
                if (this.frame == null)
                {
                    this.frame = new Frame();
                    this.RegisterFrameEvents();
                }

                return this.frame;
            }

            set
            {
                this.UnregisterFrameEvents();
                this.frame = value;
                this.RegisterFrameEvents();
            }
        }

        /// <inheritdoc/>
        public bool CanGoBack => this.Frame.CanGoBack;

        /// <inheritdoc/>
        public bool GoBack()
        {
            if (this.CanGoBack)
            {
                var viewModelBeforeNavigation = this.frame?.GetPageViewModel();
                this.frame?.GoBack();
                if (viewModelBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false)
        {
            var pageType = this.pageService?.GetPageTypeForViewModel(pageKey);
            if (pageType == null || !this.IsPageTypeValid(pageType))
            {
                return false;
            }

            if (this.Frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(this.lastParameterUsed)))
            {
                this.frame.Tag = clearNavigation;
                var viewModelBeforeNavigation = this.frame.GetPageViewModel();
                var navigated = this.frame.Navigate(pageType, parameter);
                if (navigated)
                {
                    this.lastParameterUsed = parameter;
                    if (viewModelBeforeNavigation is INavigationAware navigationAware)
                    {
                        navigationAware.OnNavigatedFrom();
                    }
                }

                return navigated;
            }

            return false;
        }

        /// <summary>
        /// Registers event handlers for the current frame.
        /// </summary>
        private void RegisterFrameEvents()
        {
            if (this.frame != null)
            {
                this.frame.Navigated += this.OnNavigated;
            }
        }

        /// <summary>
        /// Unregisters event handlers from the current frame.
        /// </summary>
        private void UnregisterFrameEvents()
        {
            if (this.frame != null)
            {
                this.frame.Navigated -= this.OnNavigated;
            }
        }

        /// <summary>
        /// Handles the <see cref="Frame.Navigated"/> event and raises the <see cref="Navigated"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                bool clearNavigation = (bool)frame.Tag;
                if (clearNavigation)
                {
                    frame.BackStack.Clear();
                }

                if (frame.Content is Page page)
                {
                    var navigatedEventArgs = new NavigatedEventArgs
                    {
                        Parameter = e.Parameter,
                        NavigationMode = e.NavigationMode,
                        SourcePageType = e.SourcePageType,
                    };
                    this.Navigated?.Invoke(sender, navigatedEventArgs);
                }
            }
        }

        /// <summary>
        /// Validates that a given type is a subclass of <see cref="Page"/>.
        /// </summary>
        /// <param name="pageType">The page type to validate.</param>
        /// <returns>True if the type is a valid Page; otherwise, false.</returns>
        private bool IsPageTypeValid(Type pageType)
        {
            return pageType.IsSubclassOf(typeof(Page));
        }
    }
}