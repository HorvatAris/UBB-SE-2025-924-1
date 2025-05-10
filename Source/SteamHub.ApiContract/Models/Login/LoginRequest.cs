// <copyright file="LoginRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Models.Login;

public class LoginRequest
{
    public string Username { get; set; }

    public string Password { get; set; }
}