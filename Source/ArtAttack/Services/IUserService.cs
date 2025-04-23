// <copyright file="IUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;

    /// <summary>
    /// Provides services related to user management, including retrieval and update operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Asynchronously retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="User"/> object,
        /// or <c>null</c> if no user with the specified ID is found.
        /// </returns>
        public Task<User?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Asynchronously retrieves all users from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="User"/> objects.</returns>
        public Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Asynchronously updates a user's information in the database.
        /// </summary>
        /// <param name="user">The user object containing the updated data.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating whether the update was successful.</returns>
        public Task<bool> UpdateUserAsync(User user);
    }
}
