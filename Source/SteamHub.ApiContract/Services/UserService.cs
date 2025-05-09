// <copyright file="UserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Proxies;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services.Interfaces;

    public class UserService : IUserService
    {
        private IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            // this.WalletBalance = walletBalance;
            // this.PointsBalance = pointsBalance;
            // this.UserRole = userRole;
            var result = new List<User>();
            var response = await this.userRepository.GetUsersAsync();
            foreach (var user in response.Users)
            {
                var currentUser = new User
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    WalletBalance = user.WalletBalance,
                    PointsBalance = user.PointsBalance,
                    UserRole = user.Role == RoleEnum.Developer
                        ? User.Role.Developer
                        : User.Role.User,
                };
                result.Add(currentUser);
            }

            return result;
        }
    }
}
