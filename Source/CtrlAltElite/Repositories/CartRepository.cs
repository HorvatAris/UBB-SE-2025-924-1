// <copyright file="CartRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using SteamStore.Constants;
using SteamStore.Data;
using SteamStore.Repositories.Interfaces;

public class CartRepository : ICartRepository
{
    private static readonly string GameIdColumn = "@game_id";
    private static readonly string UserIdColumn = "@user_id";
    private static readonly string ApprovedStatus = "Approved";
    private readonly IDataLink dataLink;
    private readonly User user;

    public CartRepository(IDataLink dataLink, User user)
    {
        this.dataLink = dataLink;
        this.user = user;
    }

    public List<Game> GetCartGames()
    {
        SqlParameter[] userIdParameters = new SqlParameter[]
        {
            new SqlParameter(UserIdColumn, this.user.UserId),
        };

        var allCartGames = this.dataLink.ExecuteReader(SqlConstants.GetAllCartGamesProcedure, userIdParameters);
        List<Game> games = new List<Game>();

        if (allCartGames != null)
        {
            foreach (DataRow row in allCartGames.Rows)
            {
                Game game = new Game
                {
                    GameId = (int)row[SqlConstants.GAMEIDCOLUMN],
                    GameTitle = (string)row[SqlConstants.NAMECOLUMN],
                    GameDescription = (string)row[SqlConstants.DESCRIPTIONCOLUMN],
                    ImagePath = (string)row[SqlConstants.IMAGEURLCOLUMN],
                    Price = Convert.ToDecimal(row[SqlConstants.PRICECOLUMN]),
                    Status = ApprovedStatus,
                };
                games.Add(game);
            }
        }

        return games;
    }

    public void AddGameToCart(Game game)
    {
        SqlParameter[] cartParameters = new SqlParameter[]
        {
            new SqlParameter(UserIdColumn, this.user.UserId),
            new SqlParameter(GameIdColumn, game.GameId),
        };

        try
        {
            this.dataLink.ExecuteNonQuery(SqlConstants.AddGameToCartProcedure, cartParameters);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }

    public void RemoveGameFromCart(Game game)
    {
        SqlParameter[] cartRemovalParameters = new SqlParameter[]
        {
            new SqlParameter(UserIdColumn, this.user.UserId),
            new SqlParameter(GameIdColumn, game.GameId),
        };

        try
        {
            this.dataLink.ExecuteNonQuery(SqlConstants.REMOVEGAMEFROMCART, cartRemovalParameters);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.Message);
        }
    }

    public float GetUserFunds()
    {
        return this.user.WalletBalance;
    }
}