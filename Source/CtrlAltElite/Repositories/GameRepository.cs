// <copyright file="GameRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using SteamStore.Data;
using SteamStore.Models;
using SteamStore.Repositories.Interfaces;
using SteamStore.Constants;

public class GameRepository : IGameRepository
{
    private static readonly int ZeroUsagesOfAGame = 0;
    private static readonly int LengthOfAnEmptyTable = 0;
    private static readonly int FirstRowIndex = 0;
    private readonly IDataLink dataLink;

    public GameRepository(IDataLink dataLink)
    {
        this.dataLink = dataLink;
    }

    public void ValidateGame(int gameId)
    {
        var validateGameParameters = new[]
        {
            new SqlParameter(SqlConstants.GameIdParameter, gameId),
        };
        this.dataLink.ExecuteNonQuery(SqlConstants.ValidateGameProcedure, validateGameParameters);
    }

    public List<Game> GetUnvalidated(int userId)
    {
        var publichedIdParameter = new[] { new SqlParameter(SqlConstants.PublisherIdParameter, userId) };

        var unvalidatedGamesData = this.dataLink.ExecuteReader(SqlConstants.GetAllUnvalidatedProcedure, publichedIdParameter);
        var currentListOfGamesList = new List<Game>();

        foreach (DataRow row in unvalidatedGamesData.Rows)
        {
            var game = new Game
            {
                GameId = (int)row[SqlConstants.GameIdColumn],
                GameTitle = (string)row[SqlConstants.GameNameColumn],
                Price = Convert.ToDecimal(row[SqlConstants.GamePriceColumn]),
                GameDescription = (string)row[SqlConstants.GameDescriptionColumn],
                ImagePath = (string)row[SqlConstants.ImageUrlColumn],
                TrailerPath = (string)row[SqlConstants.TrailerUrlColumn],
                GameplayPath = (string)row[SqlConstants.GameplayUrlColumn],
                MinimumRequirements = (string)row[SqlConstants.MinimumRequirementsColumn],
                RecommendedRequirements = (string)row[SqlConstants.RecommendedRequirementsColumn],
                Status = (string)row[SqlConstants.GameStatusColumn],
                Discount = Convert.ToDecimal(row[SqlConstants.DiscountColumn]),
                PublisherIdentifier = (int)row[SqlConstants.PublisherIdColumn],
            };
            currentListOfGamesList.Add(game);
        }

        return currentListOfGamesList;
    }

    public void DeleteGameTags(int gameId)
    {
        var gameIdParameter = new[] { new SqlParameter(SqlConstants.GameIdParameter, gameId) };
        this.dataLink.ExecuteNonQuery(SqlConstants.DeleteGameTagsProcedure, gameIdParameter);
    }

    public void DeleteGame(int gameId)
    {
        // Delete related data from all tables in the correct order to avoid foreign key constraint violations

        // 1. First delete game tags
        this.DeleteGameTags(gameId);

        // 2. Delete game reviews
        SqlParameter[] reviewParameters = { new (SqlConstants.GameIdParameter, gameId) };
        this.dataLink.ExecuteNonQuery(SqlConstants.DeleteGameReviewsProcedure, reviewParameters);

        // 3. Delete game from transaction history
        SqlParameter[] transactionParameters = { new (SqlConstants.GameIdParameter, gameId) };
        this.dataLink.ExecuteNonQuery(SqlConstants.DeleteGameTransactionsProcedure, transactionParameters);

        // 4. Delete game from user libraries
        SqlParameter[] libraryParameters = { new (SqlConstants.GameIdParameter, gameId) };
        this.dataLink.ExecuteNonQuery(SqlConstants.DeleteGameFromUserLibrariesProcedure, libraryParameters);

        // 5. delete the game itself
        SqlParameter[] gameParameters = { new (SqlConstants.GameIdParameter, gameId) };
        this.dataLink.ExecuteNonQuery(SqlConstants.DeleteGameDeveloperProcedure, gameParameters);
    }

    public List<Game> GetDeveloperGames(int userId)
    {
        SqlParameter[] developerIdParameter = { new (SqlConstants.PublisherIdParameter, userId) };
        var gamesForCurrentDeveloper = this.dataLink.ExecuteReader(SqlConstants.GetDeveloperGamesProcedure, developerIdParameter);

        return (from DataRow row in gamesForCurrentDeveloper.Rows
            select new Game
            {
                GameId = (int)row[SqlConstants.GameIdColumn],
                GameTitle = (string)row[SqlConstants.GameNameColumn],
                Price = Convert.ToDecimal(row[SqlConstants.GamePriceColumn]),
                GameDescription = (string)row[SqlConstants.GameDescriptionColumn],
                ImagePath = (string)row[SqlConstants.ImageUrlColumn],
                TrailerPath = (string)row[SqlConstants.TrailerUrlColumn],
                GameplayPath = (string)row[SqlConstants.GameplayUrlColumn],
                MinimumRequirements = (string)row[SqlConstants.MinimumRequirementsColumn],
                RecommendedRequirements = (string)row[SqlConstants.RecommendedRequirementsColumn],
                Status = (string)row[SqlConstants.GameStatusColumn],
                Discount = Convert.ToDecimal(row[SqlConstants.DiscountColumn]),
                PublisherIdentifier = (int)row[SqlConstants.PublisherIdColumn],
            }).ToList();
    }

    public void UpdateGame(int gameId, Game game)
    {
        SqlParameter[] updateGameParameters =
        {
            new (SqlConstants.GameIdParameter, gameId),
            new (SqlConstants.NameParameter, game.GameTitle),
            new (SqlConstants.PriceParameter, game.Price),
            new (SqlConstants.DescriptionParameter, game.GameDescription),
            new (SqlConstants.ImageUrlParameter, game.ImagePath),
            new (SqlConstants.TrailerUrlParameter, game.TrailerPath),
            new (SqlConstants.GameplayUrlParameter, game.GameplayPath),
            new (SqlConstants.MinimumRequirementsParameter, game.MinimumRequirements),
            new (SqlConstants.RecommendedRequirementsParameter, game.RecommendedRequirements),
            new (SqlConstants.StatusParameter, game.Status),
            new (SqlConstants.DiscountParameter, game.Discount),
            new (SqlConstants.PublisherIdParameter, game.PublisherIdentifier),
        };
        this.dataLink.ExecuteNonQuery(SqlConstants.UpdateGameProcedure, updateGameParameters);
    }

    public void RejectGame(int gameId)
    {
        SqlParameter[] rejectGameParameters = { new (SqlConstants.GameIdParameter, gameId) };
        this.dataLink.ExecuteNonQuery(SqlConstants.RejectGameProcedure, rejectGameParameters);
    }

    public void RejectGameWithMessage(int gameId, string message)
    {
        SqlParameter[] rejectGameMessageParameters =
        {
            new (SqlConstants.GameIdParameter, gameId),
            new (SqlConstants.RejectionMessageParameter, message),
        };
        this.dataLink.ExecuteNonQuery(SqlConstants.RejectGameWithMessageProcedure, rejectGameMessageParameters);
    }

    public string GetRejectionMessage(int gameId)
    {
        var gettingRejectionMessageParameters = new[] { new SqlParameter(SqlConstants.GameIdParameter, gameId) };

        var rejectedMessagesForGivenGame = this.dataLink.ExecuteReader(SqlConstants.GetRejectionMessageProcedure, gettingRejectionMessageParameters);

        if (rejectedMessagesForGivenGame.Rows.Count <= LengthOfAnEmptyTable)
        {
            return string.Empty;
        }

        return rejectedMessagesForGivenGame.Rows[FirstRowIndex][SqlConstants.RejectionMessageColumn] != DBNull.Value
            ? rejectedMessagesForGivenGame.Rows[FirstRowIndex][SqlConstants.RejectionMessageColumn].ToString()
            : string.Empty;
    }

    public void InsertGameTag(int gameId, int tagId)
    {
        var insertGameTagParameters = new SqlParameter[]
        {
            new (SqlConstants.GameIdParameter, gameId),
            new (SqlConstants.TagIdParameter, tagId),
        };

        this.dataLink.ExecuteNonQuery(SqlConstants.InsertGameTagsProcedure, insertGameTagParameters);
    }

    public bool IsGameIdInUse(int gameId)
    {
        var checkingIfGameIsInUseParameters = new SqlParameter[] { new (SqlConstants.GameIdParameter, gameId) };

        var usageResultTable = this.dataLink.ExecuteReader(SqlConstants.IsGameIdInUseProcedure, checkingIfGameIsInUseParameters);

        var gameUsageCount = Convert.ToInt32(usageResultTable.Rows[FirstRowIndex][SqlConstants.QueryResultColumn]);
        return gameUsageCount > ZeroUsagesOfAGame;
    }

    public List<Tag> GetGameTags(int gameId)
    {
        var gameIdParameters = new SqlParameter[] { new (SqlConstants.GameIdShortcutParameter, gameId) };

        var tagRows = this.dataLink.ExecuteReader(SqlConstants.GetGameTagsProcedure, gameIdParameters);

        List<Tag> tags = (from DataRow row in tagRows.Rows
                          orderby (string)row[SqlConstants.TagNameColumn]
                          select new Tag
                          {
                              TagId = (int)row[SqlConstants.TagIdColumn],
                              Tag_name = (string)row[SqlConstants.TagNameColumn],
                          }).ToList();

        return tags;
    }

    public void CreateGame(Game game)
    {
        var gameParameters = new SqlParameter[]
        {
            new (SqlConstants.GameIdParameter, game.GameId),
            new (SqlConstants.NameParameter, game.GameTitle),
            new (SqlConstants.PriceParameter, game.Price),
            new (SqlConstants.PublisherIdParameter, game.PublisherIdentifier),
            new (SqlConstants.DescriptionParameter, game.GameDescription),
            new (SqlConstants.ImageUrlParameter, game.ImagePath),
            new (SqlConstants.TrailerUrlParameter, game.TrailerPath ?? string.Empty),
            new (SqlConstants.GameplayUrlParameter, game.GameplayPath ?? string.Empty),
            new (SqlConstants.MinimumRequirementsParameter, game.MinimumRequirements),
            new (SqlConstants.RecommendedRequirementsParameter, game.RecommendedRequirements),
            new (SqlConstants.StatusParameter, game.Status),
            new (SqlConstants.DiscountParameter, game.Discount),
        };
        this.dataLink.ExecuteNonQuery(SqlConstants.InsertGameProcedure, gameParameters);
    }

    public decimal GetGameRating(int gameId)
    {
        SqlParameter[] gameRatingParameterss = { new (SqlConstants.GameIdShortcutParameter, gameId) };

        decimal? gameRating = this.dataLink.ExecuteScalar<decimal>(SqlConstants.GetGameRatingProcedure, gameRatingParameterss);
        return (decimal)gameRating;
    }

    public int GetNumberOfRecentSalesForGame(int gameId)
    {
        SqlParameter[] salesCountParameters = { new (SqlConstants.GameIdShortcutParameter, gameId) };
        int? recentSalesCount = this.dataLink.ExecuteScalar<int>(SqlConstants.GetNumberOfRecentSalesProcedure, salesCountParameters);
        return (int)recentSalesCount;
    }

    public Collection<Game> GetAllGames()
    {
        var gamesData = this.dataLink.ExecuteReader(SqlConstants.GetAllGamesProcedure);
        var gameList = new List<Game>();

        foreach (DataRow row in gamesData.Rows)
        {
            var gameId = (int)row[SqlConstants.GameIdColumn];
            var tagNames = new List<string>();
            var tags = this.GetGameTags(gameId);

            foreach (var tag in tags)
            {
                tagNames.Add(tag.Tag_name);
            }

            var game = new Game
            {
                GameId = gameId,
                PublisherIdentifier = (int)row[SqlConstants.PublisherIdColumn],
                GameTitle = (string)row[SqlConstants.GameNameColumn],
                GameDescription = (string)row[SqlConstants.GameDescriptionColumn],
                ImagePath = (string)row[SqlConstants.ImageUrlColumn],
                TrailerPath = (string)row[SqlConstants.TrailerUrlColumn],
                GameplayPath = (string)row[SqlConstants.GameplayUrlColumn],
                Price = Convert.ToDecimal(row[SqlConstants.GamePriceColumn]),
                MinimumRequirements = (string)row[SqlConstants.MinimumRequirementsColumn],
                RecommendedRequirements = (string)row[SqlConstants.RecommendedRequirementsColumn],
                Status = (string)row[SqlConstants.GameStatusColumn],
                Discount = (int)row[SqlConstants.DiscountColumn],
                Tags = tagNames.ToArray(),
                Rating = this.GetGameRating(gameId),
                NumberOfRecentPurchases = this.GetNumberOfRecentSalesForGame(gameId),
                TrendingScore = Game.NOTCOMPUTED,
                TagScore = Game.NOTCOMPUTED,
            };

            gameList.Add(game);
        }

        return new Collection<Game>(gameList);
    }
}