// <copyright file="User.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Models.User
{
    public class User
    {
        public User()
        {
        }

        public User(int userIdentifier, string name, string email, float walletBalance, float pointsBalance, Role userRole)
        {
            UserId = userIdentifier;
            UserName = name;
            Email = email;
            WalletBalance = walletBalance;
            PointsBalance = pointsBalance;
            UserRole = userRole;
        }

        public enum Role
        {
            /// <summary>
            /// User is a developer.
            /// </summary>
            Developer,

            /// <summary>
            /// User is not a developer.
            /// </summary>
            User,
        }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public float WalletBalance { get; set; }

        public float PointsBalance { get; set; }

        public Role UserRole { get; set; }
    }
}