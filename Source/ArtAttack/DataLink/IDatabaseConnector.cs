// <copyright file="IDatabaseConnector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.DataLink
{
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Interface for the database connector.
    /// </summary>
    public interface IDatabaseConnector
    {
        /// <summary>
        /// Gets the current SQL connection if open or creates and returns a new one if it's null or closed.
        /// </summary>
        /// <returns>The current or newly initialized <see cref="SqlConnection"/>.</returns>
        SqlConnection GetConnection();

        /// <summary>
        /// Always returns a new instance of <see cref="SqlConnection"/>.
        /// </summary>
        /// <returns>A new <see cref="SqlConnection"/> object.</returns>
        SqlConnection GetNewConnection();

        /// <summary>
        /// Asynchronously opens the connection if it is not already open.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task OpenConnectionAsync();

        /// <summary>
        /// Closes the connection if it is not already closed.
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Gets the image path for an item.
        /// </summary>
        /// <param name="item"> Item for which the image path is returned. </param>
        /// <returns> A string representing the image path. </returns>
        string GetItemImagePath(Item item);

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns> Current user. </returns>
        User GetCurrentUser();
    }
}
