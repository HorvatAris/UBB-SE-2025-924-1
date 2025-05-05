// <copyright file="UserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.ServiceProxies;
    using CtrlAltElite.Services.Interfaces;
    using SteamHub.ApiContract.Models.User;
    using Windows.Networking.Proximity;

    public class UserService : IUserService
    {
        private IUserServiceProxy userServiceProxy;

        public UserService(IUserServiceProxy userServiceProxy)
        {
            this.userServiceProxy = userServiceProxy;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            // this.WalletBalance = walletBalance;
            // this.PointsBalance = pointsBalance;
            // this.UserRole = userRole;
            var result = new List<User>();
            var response = await this.userServiceProxy.GetUsersAsync();
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
