// <copyright file="Game.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SteamHub.Api.Entities;

public class Game
{
    public const decimal NOTCOMPUTED = -111111;

    public Game()
    {
    }

    public int Identifier { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string? MinimumRequirements { get; set; }

    public string? RecommendedRequirements { get; set; }

    [Required]
    public GameStatus Status { get; set; }
    
    public string? RejectMessage { get; set; }

    public ISet<Tag> Tags { get; set; }

    public decimal Rating { get; set; }

    public int NumberOfRecentPurchases { get; set; }

    public decimal TrendingScore { get; set; } = NOTCOMPUTED;

    public string? TrailerPath { get; set; }

    public string? GameplayPath { get; set; }

    public decimal Discount { get; set; }

    public decimal TagScore { get; set; } = NOTCOMPUTED;

    [Required]
    public int PublisherIdentifier { get; set; }
    
}

public enum GameStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
}