// <copyright file="User.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Models.User
{
    public partial class User : IUserDetails
    {
        public User()
        {
        }

        public User(int userIdentifier, string name, string email, float walletBalance, float pointsBalance, UserRole userRole)
        {
            UserId = userIdentifier;
            UserName = name;
            Email = email;
            WalletBalance = walletBalance;
            PointsBalance = pointsBalance;
            UserRole = userRole;
        } 
        
        public User(IUserDetails userDetails)
        {
            UserId = userDetails.UserId;
            UserName = userDetails.UserName;
            Email = userDetails.Email;
            WalletBalance = userDetails.WalletBalance;
            PointsBalance = userDetails.PointsBalance;
            UserRole = userDetails.UserRole;
        }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public float WalletBalance { get; set; }

        public float PointsBalance { get; set; }

        public UserRole UserRole { get; set; }
    }
}