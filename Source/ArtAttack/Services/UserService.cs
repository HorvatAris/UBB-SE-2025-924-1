// <copyright file="UserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;
    using Steampunks.Repository.GameRepo;
    using Steampunks.Repository.UserRepository;

    /// <summary>
    /// Provides services related to user management, including retrieval and update operations.
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// The user repository used to perform user-related data operations.
        /// </summary>
        /// <remarks>
        /// This field is initialized in the constructor and is responsible for interacting with the user-related
        /// persistence logic, abstracting the database access layer.
        /// </remarks>
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class with the specified database connector.
        /// </summary>
        /// <param name="userRepository">The database connector used to interact with the data store.</param>
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Asynchronously retrieves all users from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="User"/> objects.</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Task.Run(() => this.userRepository.GetAllUsersAsync());
        }

        /// <summary>
        /// Asynchronously retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="User"/> object,
        /// or <c>null</c> if no user with the specified ID is found.
        /// </returns>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await Task.Run(() => this.userRepository.GetUserByIdAsync(userId));
        }

        /// <summary>
        /// Asynchronously updates a user's information in the database.
        /// </summary>
        /// <param name="user">The user object containing the updated data.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating whether the update was successful.</returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            // return await Task.Run(() => this.userRepository.UpdateUserAsync(user));
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return await this.userRepository.UpdateUserAsync(user);
        }
    }
}