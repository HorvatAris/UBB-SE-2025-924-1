// <copyright file="User.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Models
{
    public class User
    {
        public User()
        {
        }

        public User(int userIdentifier, string name, string email, float walletBalance, float pointsBalance, Role userRole)
        {
            this.UserId = userIdentifier;
            this.UserName = name;
            this.Email = email;
            this.WalletBalance = walletBalance;
            this.PointsBalance = pointsBalance;
            this.UserRole = userRole;
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