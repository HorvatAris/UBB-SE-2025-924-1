// <copyright file="IUserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Service class for accessing and managing Game data.
// </summary>
// Refactored by Team ArtAttack, 2025
namespace Steampunks.Repository.UserRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Defines methods for accessing and manipulating user data.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Asynchronously retrieves all users from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="User"/> objects.</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Asynchronously retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="User"/> object,
        /// or <c>null</c> if no user with the specified ID is found.
        /// </returns>
        Task<User?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Asynchronously updates the specified user's data in the database.
        /// </summary>
        /// <param name="user">The user object containing updated information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result indicates whether the update was successful.
        /// </returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Asynchronously inserts test users into the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task InsertTestUsersAsync();
    }
}
