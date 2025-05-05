// <copyright file="GameMapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using SteamHub.ApiContract.Models.Game;

namespace CtrlAltElite.Services;

public class GameMapper
{
    public static Game MapToGame(GameDetailedResponse game)
    {
        return new Game
        {
            GameId = game.Identifier,
            Status = game.Status.ToString(),
            GameDescription = game.Description,
            ImagePath = game.ImagePath,
            GameTitle = game.Name,
            Price = game.Price,
            RecommendedRequirements = game.RecommendedRequirements,
            MinimumRequirements = game.MinimumRequirements,
            Discount = game.Discount,
            NumberOfRecentPurchases = game.NumberOfRecentPurchases,
            Rating = game.Rating,
            TrailerPath = game.TrailerPath,
            GameplayPath = game.GameplayPath,
            PublisherIdentifier = game.PublisherUserIdentifier,
            Tags = game.Tags?.Select(tag => tag.TagName).ToArray(),
            TagScore = Game.NOTCOMPUTED,
        };
    }

}