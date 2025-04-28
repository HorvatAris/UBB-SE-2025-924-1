// <copyright file="UserGameService.cs" company="PlaceholderCompany">
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
using SteamStore.Constants;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Repositories.Interfaces;
using SteamStore.Services.Interfaces;

public class UserGameService : IUserGameService
{
    private const int InitialValueForLastEarnedPoints = 0;
    private const int ResetValueForNumberOfUserGamesWithTag = 0;
    private const int NumberOfFavouriteTagsToTake = 3;
    private const int StartingIndexValue = 0;
    private const int InitialTagScore = 0;
    private const int TagScoreMultiplierNumerator = 1;
    private const decimal TagScoreMultiplierDenominator = 3m;
    private const decimal WeightedScoreMultiplier = 0.5m;
    private const int NumberOfSortegGamesShown = 10;
    private const decimal MinimumValueForOverwhelminglyPositive = 4.5m;
    private const decimal MinimumValueForVeryPositive = 4m;
    private const decimal MinimumValueForMixed = 2m;
    private const int ValueToDecrementPositionWith = 1;
    private const int ValueToIncrementPositionWith = 1;

    public IUserGameRepository UserGameRepository { get; set; }

    public IGameServiceProxy GameServiceProxy { get; set; }

    public ITagRepository TagRepository { get; set; }

    // Property to track points earned in the last purchase
    public int LastEarnedPoints { get; private set; }

    public void RemoveGameFromWishlist(Game game)
    {
        this.UserGameRepository.RemoveGameFromWishlist(game);
    }

    public void AddGameToWishlist(Game game)
    {
        try
        {
            // Check if game is already purchased
            if (this.IsGamePurchased(game))
            {
                throw new Exception(string.Format(ExceptionMessages.GameAlreadyOwned, game.GameTitle));
            }

            this.UserGameRepository.AddGameToWishlist(game);
        }
        catch (Exception exception)
        {
            // Clean up the error message
            string message = exception.Message;
            if (message.Contains("ExecuteNonQuery"))
            {
                message = string.Format(ExceptionMessages.GameAlreadyInWishlist, game.GameTitle);
            }

            throw new Exception(message);
        }
    }

    public void PurchaseGames(List<Game> games)
    {
        // Reset points counter
        this.LastEarnedPoints = InitialValueForLastEarnedPoints;

        // Track user's points before purchase
        float pointsBalanceBefore = this.UserGameRepository.GetUserPointsBalance();

        // Purchase games
        foreach (var game in games)
        {
            this.UserGameRepository.AddGameToPurchased(game);
            this.UserGameRepository.RemoveGameFromWishlist(game);
        }

        // Calculate earned points by comparing balances
        float pointsBalanceAfter = this.UserGameRepository.GetUserPointsBalance();
        this.LastEarnedPoints = (int)(pointsBalanceAfter - pointsBalanceBefore);
    }

    public void ComputeNoOfUserGamesForEachTag(Collection<Tag> all_tags)
    {
        var user_games = this.UserGameRepository.GetAllUserGames();

        // Manually build the dictionary instead of using ToDictionary
        Dictionary<string, Tag> tagsDictionary = new Dictionary<string, Tag>();
        foreach (var tag in all_tags)
        {
            if (!tagsDictionary.ContainsKey(tag.Tag_name))
            {
                tagsDictionary.Add(tag.Tag_name, tag);
            }
        }

        foreach (var tag in tagsDictionary.Values)
        {
            tag.NumberOfUserGamesWithTag = ResetValueForNumberOfUserGamesWithTag;
        }

        foreach (var user_game in user_games)
        {
            foreach (string tag_name in user_game.Tags)
            {
                if (tagsDictionary.ContainsKey(tag_name))
                {
                    tagsDictionary[tag_name].NumberOfUserGamesWithTag++;
                }
            }
        }
    }

    public Collection<Tag> GetFavoriteUserTags()
    {
        var allTags = this.TagRepository.GetAllTags();
        this.ComputeNoOfUserGamesForEachTag(allTags);

        List<Tag> sortedTags = new List<Tag>(allTags);

        for (int currentIndex = StartingIndexValue; currentIndex < sortedTags.Count - ValueToDecrementPositionWith; currentIndex++)
        {
            for (int comparisonIndex = currentIndex + ValueToIncrementPositionWith; comparisonIndex < sortedTags.Count; comparisonIndex++)
            {
                if (sortedTags[comparisonIndex].NumberOfUserGamesWithTag > sortedTags[currentIndex].NumberOfUserGamesWithTag)
                {
                    var tagToSwap = sortedTags[currentIndex];
                    sortedTags[currentIndex] = sortedTags[comparisonIndex];
                    sortedTags[comparisonIndex] = tagToSwap;
                }
            }
        }

        List<Tag> topTags = new List<Tag>();
        for (int tagIndex = StartingIndexValue; tagIndex < sortedTags.Count && tagIndex < NumberOfFavouriteTagsToTake; tagIndex++)
        {
            topTags.Add(sortedTags[tagIndex]);
        }

        return new Collection<Tag>(topTags);
    }

    public void ComputeTagScoreForGames(Collection<Game> games)
    {
        var favorite_tags = this.GetFavoriteUserTags();
        foreach (var game in games)
        {
            game.TagScore = InitialTagScore;
            foreach (var tag in favorite_tags)
            {
                if (game.Tags.Contains(tag.Tag_name))
                {
                    game.TagScore += tag.NumberOfUserGamesWithTag;
                }
            }

            game.TagScore = game.TagScore * (TagScoreMultiplierNumerator / TagScoreMultiplierDenominator);
        }
    }

    public void ComputeTrendingScores(Collection<Game> games)
    {
        var maxRecentSales = games.Max(game => game.NumberOfRecentPurchases);
        foreach (var game in games)
        {
            game.TrendingScore = Convert.ToDecimal(game.NumberOfRecentPurchases) / maxRecentSales;
        }
    }

    public async Task<Collection<Game>> GetRecommendedGames()
    {
        var games = await this.GameServiceProxy.GetGamesAsync(
            new GetGamesRequest());
        var allGames = new Collection<Game>(games.Select(GameMapper.MapToGame).ToList());
        this.ComputeTrendingScores(allGames);
        this.ComputeTagScoreForGames(allGames);
        
        List<Game> sortedGames = new List<Game>(allGames);

        // Manual sorting based on weighted score
        for (int currentIndex = StartingIndexValue; currentIndex < sortedGames.Count - 1; currentIndex++)
        {
            for (int comparisonIndex = currentIndex + 1; comparisonIndex < sortedGames.Count; comparisonIndex++)
            {
                decimal currentScore = (sortedGames[currentIndex].TagScore * WeightedScoreMultiplier) + (sortedGames[currentIndex].TrendingScore * WeightedScoreMultiplier);
                decimal comparisonScore = (sortedGames[comparisonIndex].TagScore * WeightedScoreMultiplier) + (sortedGames[comparisonIndex].TrendingScore * WeightedScoreMultiplier);

                if (comparisonScore > currentScore)
                {
                    Game gameToSwap = sortedGames[currentIndex];
                    sortedGames[currentIndex] = sortedGames[comparisonIndex];
                    sortedGames[comparisonIndex] = gameToSwap;
                }
            }
        }

        // Take top games
        List<Game> recommendedGames = new List<Game>();
        int limit = sortedGames.Count < NumberOfSortegGamesShown ? sortedGames.Count : NumberOfSortegGamesShown;
        for (int gameIndex = StartingIndexValue; gameIndex < limit; gameIndex++)
        {
            recommendedGames.Add(sortedGames[gameIndex]);
        }

        return new Collection<Game>(recommendedGames);
    }

    public Collection<Game> GetWishListGames()
    {
        return this.UserGameRepository.GetWishlistGames();
    }

    public Collection<Game> SearchWishListByName(string searchText)
    {
        List<Game> allWishListGames = this.UserGameRepository.GetWishlistGames().ToList();
        List<Game> matchingGames = new List<Game>();

        foreach (Game game in allWishListGames)
        {
            if (game.GameTitle != null && game.GameTitle.ToLower().Contains(searchText.ToLower()))
            {
                matchingGames.Add(game);
            }
        }

        return new Collection<Game>(matchingGames);
    }

    public Collection<Game> FilterWishListGames(string criteria)
    {
        Collection<Game> games = this.UserGameRepository.GetWishlistGames();
        Collection<Game> filteredGames = new Collection<Game>();

        bool isKnownCriteria = criteria == FilterCriteria.OVERWHELMINGLYPOSITIVE ||
                               criteria == FilterCriteria.VERYPOSITIVE ||
                               criteria == FilterCriteria.MIXED ||
                               criteria == FilterCriteria.NEGATIVE;

        if (!isKnownCriteria)
        {
            // If the criteria is not recognized, return the full list
            return games;
        }

        foreach (Game game in games)
        {
            if (criteria == FilterCriteria.OVERWHELMINGLYPOSITIVE && game.Rating >= MinimumValueForOverwhelminglyPositive)
            {
                filteredGames.Add(game);
            }
            else if (criteria == FilterCriteria.VERYPOSITIVE &&
                     game.Rating >= MinimumValueForVeryPositive &&
                     game.Rating < MinimumValueForOverwhelminglyPositive)
            {
                filteredGames.Add(game);
            }
            else if (criteria == FilterCriteria.MIXED &&
                     game.Rating >= MinimumValueForMixed &&
                     game.Rating < MinimumValueForVeryPositive)
            {
                filteredGames.Add(game);
            }
            else if (criteria == FilterCriteria.NEGATIVE &&
                     game.Rating < MinimumValueForMixed)
            {
                filteredGames.Add(game);
            }
        }

        return filteredGames;
    }

    public bool IsGamePurchased(Game game)
    {
        return this.UserGameRepository.IsGamePurchased(game);
    }

    public Collection<Game> SortWishListGames(string criteria, bool ascending)
    {
        Collection<Game> gamesCollection = this.UserGameRepository.GetWishlistGames();
        List<Game> games = new List<Game>();

        foreach (var game in gamesCollection)
        {
            games.Add(game);
        }

        if (criteria == FilterCriteria.PRICE)
        {
            if (ascending)
            {
                games.Sort(this.CompareByPriceAscending);
            }
            else
            {
                games.Sort(this.CompareByPriceDescending);
            }
        }
        else if (criteria == FilterCriteria.RATING)
        {
            if (ascending)
            {
                games.Sort(this.CompareByRatingAscending);
            }
            else
            {
                games.Sort(this.CompareByRatingDescending);
            }
        }
        else if (criteria == FilterCriteria.DISCOUNT)
        {
            if (ascending)
            {
                games.Sort(this.CompareByDiscountAscending);
            }
            else
            {
                games.Sort(this.CompareByDiscountDescending);
            }
        }
        else
        {
            if (ascending)
            {
                games.Sort(this.CompareByNameAscending);
            }
            else
            {
                games.Sort(this.CompareByNameDescending);
            }
        }

        return new Collection<Game>(games);
    }

    private int CompareByPriceAscending(Game firstGame, Game secondGame)
    {
        return firstGame.Price.CompareTo(secondGame.Price);
    }

    private int CompareByPriceDescending(Game firstGame, Game secondGame)
    {
        return secondGame.Price.CompareTo(firstGame.Price);
    }

    private int CompareByRatingAscending(Game firstGame, Game secondGame)
    {
        return firstGame.Rating.CompareTo(secondGame.Rating);
    }

    private int CompareByRatingDescending(Game firstGame, Game secondGame)
    {
        return secondGame.Rating.CompareTo(firstGame.Rating);
    }

    private int CompareByDiscountAscending(Game firstGame, Game secondGame)
    {
        return firstGame.Discount.CompareTo(secondGame.Discount);
    }

    private int CompareByDiscountDescending(Game firstGame, Game secondGame)
    {
        return secondGame.Discount.CompareTo(firstGame.Discount);
    }

    private int CompareByNameAscending(Game firstGame, Game secondGame)
    {
        return string.Compare(firstGame.GameTitle, secondGame.GameTitle, StringComparison.OrdinalIgnoreCase);
    }

    private int CompareByNameDescending(Game firstGame, Game secondGame)
    {
        return string.Compare(secondGame.GameTitle, firstGame.GameTitle, StringComparison.OrdinalIgnoreCase);
    }
}