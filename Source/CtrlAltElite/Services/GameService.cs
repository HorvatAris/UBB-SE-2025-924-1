// <copyright file="GameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CtrlAltElite.ServiceProxies;
using CtrlAltElite.Services;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Services.Interfaces;


namespace SteamStore.Services;

public class GameService : IGameService
{
    private const int MinimumTrendingDivider = 1;
    private const decimal NoTrendingScore = 0m;
    private const int NumberOfSimilarGamesToTake = 3;
    private static int lengthOfEmptyList = 0;
    private static int initializingValueForAMaxim = 0;
    private static int treasholdForDiscount = 0;
    private static int startingValueOfIndex = 0;
    private static int incrementingValue = 1;
    private static int numberOfGamesToTake = 10;

    public IGameServiceProxy GameServiceProxy { get; set; }

   //public ITagRepository TagRepository { get; set; }
    public ITagServiceProxy TagRepository { get; set; }

    public async Task<Collection<Game>> GetAllGames()
    {
        var games = await this.GameServiceProxy.GetGamesAsync(new GetGamesRequest());
        return new Collection<Game>(games.Select(GameMapper.MapToGame).ToList());
    }

    public async Task<Collection<Tag>> GetAllTags()
    {
        try
        {
            var tagsResponse = await this.TagRepository.GetAllTagsAsync();
            return new Collection<Tag>(
                        tagsResponse.Tags.Select(TagMapper.MapToTag).ToList()
                    );
        }
        catch (Refit.ApiException ex)
        {
            string rawContent = ex.Content; // Removed ReadAsStringAsync as 'Content' is already a string
            Console.WriteLine("Raw API Response:");
            Console.WriteLine(rawContent);
            throw; // rethrow the exception or handle it appropriately
        }
    }



    public async Task<Collection<Tag>> GetAllGameTags(Game game)
    {
        var tagsResponse = await this.TagRepository.GetAllTagsAsync();
        var allTags = new Collection<Tag>(tagsResponse.Tags.Select(TagMapper.MapToTag).ToList());
        // Extract the result from the task
        var tagsForCurrentGame = new List<Tag>();

        foreach (var tag in allTags)
        {
            if (game.Tags.Contains(tag.Tag_name))
            {
                tagsForCurrentGame.Add(tag);
            }
        }

        return new Collection<Tag>(tagsForCurrentGame);
    }

    public async Task<Collection<Game>> SearchGames(string searchQuery)
    {
        var allGames = await GetAllGames();
        var foundGames = new List<Game>();

        foreach (var game in allGames)
        {
            if (game.GameTitle.ToLower().Contains(searchQuery.ToLower()))
            {
                foundGames.Add(game);
            }
        }

        return new Collection<Game>(foundGames);
    }

    public async Task<Collection<Game>> FilterGames(
        int minimumRating,
        int minimumPrice,
        int maximumPrice,
        string[] tags)
    {
        if (tags == null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        var filteredGames = new List<Game>();

        var allGames = await this.GetAllGames();
        foreach (var game in allGames)
        {
            if (game.Rating >= minimumRating && game.Price >= minimumPrice && game.Price <= maximumPrice)
            {
                bool hasAllTags = true;

                // If there are tags, check if the game has all the tags
                if (tags.Length > lengthOfEmptyList)
                {
                    foreach (var tag in tags)
                    {
                        if (!game.Tags.Contains(tag))
                        {
                            hasAllTags = false;
                            break; // No need to check further if one tag is missing
                        }
                    }
                }

                // If game has all tags or no tags were provided, add to the filtered list
                if (hasAllTags)
                {
                    filteredGames.Add(game);
                }
            }
        }

        return new Collection<Game>(filteredGames);
    }

    public void ComputeTrendingScores(Collection<Game> games)
    {
        int maximumRecentSales = initializingValueForAMaxim;

        foreach (var game in games)
        {
            if (game.NumberOfRecentPurchases > maximumRecentSales)
            {
                maximumRecentSales = game.NumberOfRecentPurchases;
            }
        }

        foreach (var game in games)
        {
            game.TrendingScore = maximumRecentSales < MinimumTrendingDivider
                ? NoTrendingScore
                : Convert.ToDecimal(game.NumberOfRecentPurchases) / maximumRecentSales;
        }
    }

    public async Task<Collection<Game>> GetTrendingGames()
    {
        var allGames = await this.GetAllGames();
        return this.GetSortedAndFilteredVideoGames(allGames);
    }

    public async Task<Collection<Game>> GetDiscountedGames()
    {
        var allGames = await this.GetAllGames();
        var discountedGames = new List<Game>();

        foreach (var game in allGames)
        {
            if (game.Discount > treasholdForDiscount)
            {
                discountedGames.Add(game);
            }
        }

        return this.GetSortedAndFilteredVideoGames(new Collection<Game>(discountedGames));
    }

    public async Task<List<Game>> GetSimilarGames(int gameId)
    {
        var randomGenerator = new Random(DateTime.Now.Millisecond);
        var allGames = await GetAllGames();
        var similarGames = new List<Game>();

        // Filter games with different identifiers
        foreach (var game in allGames)
        {
            if (game.GameId != gameId)
            {
                similarGames.Add(game);
            }
        }

        // Shuffle the list
        for (int currentIndex = startingValueOfIndex; currentIndex < similarGames.Count; currentIndex++)
        {
            var randomIndex = randomGenerator.Next(
                currentIndex,
                similarGames.Count); // Get a random index from currentIndex to the end of the list
            var temporaryGame = similarGames[currentIndex];
            similarGames[currentIndex] = similarGames[randomIndex];
            similarGames[randomIndex] = temporaryGame;
        }

        // Return the first 3 games
        return similarGames.Take(NumberOfSimilarGamesToTake).ToList();
    }

    public async Task<Game> GetGameById(int gameId)
    {
        return GameMapper.MapToGame(await this.GameServiceProxy.GetGameByIdAsync(gameId));
    }

    private Collection<Game> GetSortedAndFilteredVideoGames(Collection<Game> games)
    {
        // Compute the trending scores for all the games
        this.ComputeTrendingScores(games);

        // Create a list to hold the sorted games
        List<Game> sortedGames = new List<Game>();

        // Manually sort the games by descending trending score
        for (int currentIndex = startingValueOfIndex; currentIndex < games.Count; currentIndex++)
        {
            for (int comparisonIndex = currentIndex + incrementingValue;
                 comparisonIndex < games.Count;
                 comparisonIndex++)
            {
                if (games[currentIndex].TrendingScore < games[comparisonIndex].TrendingScore)
                {
                    // Swap the games
                    var temporaryGame = games[currentIndex];
                    games[currentIndex] = games[comparisonIndex];
                    games[comparisonIndex] = temporaryGame;
                }
            }
        }

        // Take the top 10 games after sorting
        for (int topGamesIndex = startingValueOfIndex;
             topGamesIndex < numberOfGamesToTake && topGamesIndex < games.Count;
             topGamesIndex++)
        {
            sortedGames.Add(games[topGamesIndex]);
        }

        return new Collection<Game>(sortedGames);
    }
}