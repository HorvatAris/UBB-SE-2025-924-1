// <copyright file="UserGameRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using SteamStore.Constants;
using SteamStore.Data;
using SteamStore.Repositories.Interfaces;
using Windows.Gaming.Input;

public class UserGameRepository : IUserGameRepository
{
    private const int FirstRowIndex = 0;
    private const int DefaultValueOfOwners = 0;
    private const int PointsPerDollar = 121;
    private const int ValueOfNotBeingPurchased = 0;
    private const int ZeroRowsCount = 0;
    private const string OwnerCountColumn = "OwnerCount";
    private User user;
    private IDataLink dataLink;

    public UserGameRepository(IDataLink data, User user)
    {
        this.user = user;
        this.dataLink = data;
    }

    public bool IsGamePurchased(Game game)
    {
        SqlParameter[] gamePurchasedParameters = new SqlParameter[]
        {
            new SqlParameter(SqlConstants.GameIdParameter, game.Identifier),
            new SqlParameter(SqlConstants.UserIdParameter, this.user.UserIdentifier),
        };
        try
        {
            return this.dataLink.ExecuteScalar<int>(SqlConstants.IsGamePurchasedProcedure, gamePurchasedParameters) > ValueOfNotBeingPurchased;
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }

    public void RemoveGameFromWishlist(Game game)
    {
        SqlParameter[] removeGamesParameters = new SqlParameter[]
        {
            new SqlParameter(SqlConstants.UserIdParameter, this.user.UserIdentifier),
            new SqlParameter(SqlConstants.GameIdParameter, game.Identifier),
        };

        try
        {
            this.dataLink.ExecuteNonQuery(SqlConstants.RemoveGameFromWishlistProcedure, removeGamesParameters);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }

    public void AddGameToPurchased(Game game)
    {
        SqlParameter[] purchaseGameParameters = new SqlParameter[]
        {
            new SqlParameter(SqlConstants.UserIdParameter, this.user.UserIdentifier),
            new SqlParameter(SqlConstants.GameIdParameter, game.Identifier),
        };

        try
        {
            if (Convert.ToDecimal(this.user.WalletBalance) < game.Price)
            {
                throw new Exception("Insufficient funds");
            }

            this.dataLink.ExecuteNonQuery(SqlConstants.AddGameToPurchasedGamesProcedure, purchaseGameParameters);
            this.user.WalletBalance -= (float)game.Price;

            // Calculate and add points (121 points for every $1 spent)
            this.AddPointsForPurchase((float)game.Price);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }

    public void AddGameToWishlist(Game game)
    {
        SqlParameter[] addGameToWishlistParameters = new SqlParameter[]
        {
            new SqlParameter(SqlConstants.UserIdParameter, this.user.UserIdentifier),
            new SqlParameter(SqlConstants.GameIdParameter, game.Identifier),
        };

        try
        {
            this.dataLink.ExecuteNonQuery(SqlConstants.AddGameToWishlistProcedure, addGameToWishlistParameters);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }

    public string[] GetGameTags(int gameId)
    {
        SqlParameter[] gameTagsParameters = new SqlParameter[]
        {
            new SqlParameter(SqlConstants.GameIdShortcutParameter, gameId),
        };

        try
        {
            DataTable gameTagsTable = this.dataLink.ExecuteReader(SqlConstants.GetGameTagsProcedure, gameTagsParameters);
            List<string> tags = new List<string>();

            if (gameTagsTable != null)
            {
                foreach (DataRow row in gameTagsTable.Rows)
                {
                    tags.Add((string)row[SqlConstants.TagNameColumn]);
                }
            }

            return tags.ToArray();
        }
        catch (Exception exception)
        {
            throw new Exception($"Error getting tags for game {gameId}: {exception.Message}");
        }
    }

    public int GetGameOwnerCount(int gameId)
    {
        var ownerCountParameters = new SqlParameter[] { new (SqlConstants.GameIdParameter, gameId) };
        var result = this.dataLink.ExecuteReader(SqlConstants.GetGameOwnerCountProcedure, ownerCountParameters);

        return result is { Rows.Count: > ZeroRowsCount } ? Convert.ToInt32(result.Rows[FirstRowIndex][OwnerCountColumn]) : DefaultValueOfOwners;
    }

    public Collection<Game> GetAllUserGames()
    {
        SqlParameter[] allUsersParameters = new SqlParameter[]
        {
            new SqlParameter(SqlConstants.UserIdentifierParameter, this.user.UserIdentifier),
        };

        var allUserGames = this.dataLink.ExecuteReader(SqlConstants.GetUserGamesProcedure, allUsersParameters);
        List<Game> games = new List<Game>();

        if (allUserGames != null)
        {
            foreach (DataRow row in allUserGames.Rows)
            {
                Game game = new Game
                {
                    Identifier = (int)row[SqlConstants.GameIdColumn],
                    Name = (string)row[SqlConstants.GameNameColumn],
                    Description = (string)row[SqlConstants.DescriptionIdColumnWithCapitalLetter],
                    ImagePath = (string)row[SqlConstants.ImageUrlColumn],
                    Price = Convert.ToDecimal(row[SqlConstants.GamePriceColumn]),
                    MinimumRequirements = (string)row[SqlConstants.MinimumRequirementsColumn],
                    RecommendedRequirements = (string)row[SqlConstants.RecommendedRequirementsColumn],
                    Status = (string)row[SqlConstants.GameStatusColumn],
                    Tags = this.GetGameTags((int)row[SqlConstants.GameIdColumn]),
                    TrendingScore = Game.NOTCOMPUTED,
                };
                games.Add(game);
            }
        }

        return new Collection<Game>(games);
    }

    public void AddPointsForPurchase(float purchaseAmount)
    {
        try
        {
            // Calculate points (121 points for every $1 spent)
            int earnedPoints = (int)(purchaseAmount * PointsPerDollar);

            // Update user's point balance
            this.user.PointsBalance += earnedPoints;

            // Update in database
            SqlParameter[] addingPointsParameter = new SqlParameter[]
            {
                new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, this.user.UserIdentifier),
                new SqlParameter(SqlConstants.PointBalanceParameter, this.user.PointsBalance),
            };

            this.dataLink.ExecuteNonQuery(SqlConstants.UpdateUserPointBalance, addingPointsParameter);
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed to add points for purchase: {exception.Message}");
        }
    }

    public float GetUserPointsBalance()
    {
        // Simply return the user's current points balance from the model
        return this.user.PointsBalance;
    }

    public Collection<Game> GetWishlistGames()
    {
        SqlParameter[] wishlistGamesParameters = new SqlParameter[]
        {
            new SqlParameter(SqlConstants.UserIdParameter, this.user.UserIdentifier),
        };

        var wishlistGamesData = this.dataLink.ExecuteReader(SqlConstants.GetWishlistGamesProcedure, wishlistGamesParameters);
        List<Game> gamesInWishlist = new List<Game>();

        if (wishlistGamesData != null)
        {
            foreach (DataRow row in wishlistGamesData.Rows)
            {
                Game game = new Game
                {
                    Identifier = (int)row[SqlConstants.GameIdColumn],
                    Name = (string)row[SqlConstants.GameNameColumn],
                    Description = (string)row[SqlConstants.DescriptionIdColumnWithCapitalLetter],
                    ImagePath = (string)row[SqlConstants.ImageUrlColumn],
                    Price = Convert.ToDecimal(row[SqlConstants.GamePriceColumn]),
                    MinimumRequirements = (string)row[SqlConstants.MinimumRequirementsColumn],
                    RecommendedRequirements = (string)row[SqlConstants.RecommendedRequirementsColumn],
                    Status = (string)row[SqlConstants.GameStatusColumn],
                    Rating = Convert.ToDecimal(row[SqlConstants.RatingColumn]),
                    Discount = Convert.ToDecimal(row[SqlConstants.DiscountColumn]),
                    Tags = this.GetGameTags((int)row[SqlConstants.GameIdColumn]),
                };
                gamesInWishlist.Add(game);
            }
        }

        return new Collection<Game>(gamesInWishlist);
    }
}