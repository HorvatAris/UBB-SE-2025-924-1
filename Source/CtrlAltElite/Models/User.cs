// <copyright file="User.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

public class User
{
    public User()
    {
    }

    public User(int userIdentifier, string name, string email, float walletBalance, float pointsBalance, Role userRole)
    {
        this.UserIdentifier = userIdentifier;
        this.Name = name;
        this.Email = email;
        this.WalletBalance = walletBalance;
        this.PointsBalance = pointsBalance;
        this.UserRole = userRole;
    }

    public enum Role
    {
        Developer,
        User,
    }

    public int UserIdentifier { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public float WalletBalance { get; set; }

    public float PointsBalance { get; set; }

    public Role UserRole { get; set; }
}