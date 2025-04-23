﻿// <copyright file="Game.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Runtime.Intrinsics.X86;
using Windows.Devices.Pwm;

public class Game
{
    public const decimal NOTCOMPUTED = -111111;

    public Game()
    {
    }

    public int Identifier { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ImagePath { get; set; }

    public decimal Price { get; set; }

    public string MinimumRequirements { get; set; }

    public string RecommendedRequirements { get; set; }

    public string Status { get; set; }

    public string[] Tags { get; set; }

    public decimal Rating { get; set; }

    public int NumberOfRecentPurchases { get; set; }

    public decimal TrendingScore { get; set; }

    public string TrailerPath { get; set; }

    public string GameplayPath { get; set; }

    public decimal Discount { get; set; }

    public decimal TagScore { get; set; }

    public int PublisherIdentifier { get; set; }
}
