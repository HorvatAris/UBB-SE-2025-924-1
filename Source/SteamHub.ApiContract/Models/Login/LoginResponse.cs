// <copyright file="LoginResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Models.Login;

public class LoginResponse
{
    public int UserId { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string UserRole { get; set; }
    public float PointsBalance { get; set; }
    public float WalletBalance { get; set; }
}