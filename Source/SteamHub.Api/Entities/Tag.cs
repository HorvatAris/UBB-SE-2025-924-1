// <copyright file="Tag.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>


using System.Text.Json.Serialization;

namespace SteamHub.Api.Entities;

public class Tag
{
    public const int NOTCOMPUTED = -11111;

    public int TagId { get; set; }

    public string Tag_name { get; set; }

    [JsonIgnore]
    public int NumberOfUserGamesWithTag { get; set; }
}