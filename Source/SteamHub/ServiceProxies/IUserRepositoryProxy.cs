// <copyright file="IUserServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ServiceProxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Refit;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Repositories;

    public interface IUserRepositoryProxy : IUserRepository
    {
        [Put("/api/Users/{id}")]
        Task UpdateUserAsync(int id, [Body] UpdateUserRequest request);

        [Get("/api/users")]
        Task<GetUsersResponse?> GetUsersAsync();

        [Get("/api/users/{id}")]
        Task<UserResponse?> GetUserByIdAsync(int id);
    }
}
