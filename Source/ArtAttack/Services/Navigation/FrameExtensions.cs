// <copyright file="FrameExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Provides extension methods for Frame objects.
    /// </summary>
    public static class FrameExtensions
    {
        /// <summary>
        /// Gets the ViewModel from the current page if it exposes one.
        /// </summary>
        /// <param name="frame">The frame to extract the ViewModel from.</param>
        /// <returns>The ViewModel instance, or null if not found.</returns>
        public static object? GetPageViewModel(this Frame frame)
            => frame?.Content?.GetType().GetProperty("ViewModel")?.GetValue(frame.Content);
    }
}