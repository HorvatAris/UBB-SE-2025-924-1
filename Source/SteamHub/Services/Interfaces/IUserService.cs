// <copyright file="IUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.Models;

    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
    }
}
