// <copyright file="DeveloperService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SteamStore.Constants;
using SteamStore.Models;
using SteamStore.Repositories;
using SteamStore.Repositories.Interfaces;
using SteamStore.Services.Interfaces;
using static SteamStore.Constants.NotificationStrings;

public class DeveloperService : IDeveloperService
{
    private const int ComparingValueForPositivePrice = 0;
    private const int ComparingValueForMinimumDicount = 0;
    private const int ComparingValueForMaximumDicount = 100;
    private const int EmptyListLength = 0;
    private const string PendingState = "Pending";

    public IGameRepository GameRepository { get; set; }

    public ITagRepository TagRepository { get; set; }

    public IUserGameRepository UserGameRepository { get; set; }

    public User User { get; set; }

    public void ValidateGame(int game_id)
    {
        this.GameRepository.ValidateGame(game_id);
    }

    public Game ValidateInputForAddingAGame(string gameIdText, string name, string priceText, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string reccommendedRequirement, string discountText, IList<Tag> selectedTags)
    {
        if (string.IsNullOrWhiteSpace(gameIdText) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(priceText) ||
            string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(imageUrl) || string.IsNullOrWhiteSpace(minimumRequirement) ||
            string.IsNullOrWhiteSpace(reccommendedRequirement) || string.IsNullOrWhiteSpace(discountText))
        {
            throw new Exception(ExceptionMessages.AllFieldsRequired);
        }

        if (!int.TryParse(gameIdText, out int gameId))
        {
            throw new Exception(ExceptionMessages.InvalidGameId);
        }

        if (!decimal.TryParse(priceText, out var price) || price < ComparingValueForPositivePrice)
        {
            throw new Exception(ExceptionMessages.InvalidPrice);
        }

        if (!decimal.TryParse(discountText, out var discount) || discount < ComparingValueForMinimumDicount || discount > ComparingValueForMaximumDicount)
        {
            throw new Exception(ExceptionMessages.InvalidDiscount);
        }

        if (selectedTags == null || selectedTags.Count == EmptyListLength)
        {
            throw new Exception(ExceptionMessages.NoTagsSelected);
        }

       // var game = new Game(GameId, name,price, description, imageUrl, gameplayUrl, trailerUrl, minReq, recReq, "Pending", discount);
        var game = new Game
        {
            GameId = gameId,
            GameTitle = name,
            Price = price,
            GameDescription = description,
            ImagePath = imageUrl,
            GameplayPath = gameplayUrl,
            TrailerPath = trailerUrl,
            MinimumRequirements = minimumRequirement,
            RecommendedRequirements = reccommendedRequirement,
            Status = PendingState,
            Discount = discount,
        };
        return game;
    }

    public Game FindGameInObservableCollectionById(int gameId, ObservableCollection<Game> gameList)
    {
        foreach (Game game in gameList)
        {
            if (game.GameId == gameId)
            {
                return game;
            }
        }

        return null;
    }

    public void CreateGame(Game game)
    {
        game.PublisherIdentifier = this.User.UserId;
        this.GameRepository.CreateGame(game);
    }

    public void CreateGameWithTags(Game game, IList<Tag> selectedTags)
    {
        this.CreateGame(game);

        if (selectedTags != null && selectedTags.Count > EmptyListLength)
        {
            foreach (var tag in selectedTags)
            {
                this.InsertGameTag(game.GameId, tag.TagId);
            }
        }
    }

    public void UpdateGame(Game game)
    {
        game.PublisherIdentifier = this.User.UserId;
        this.GameRepository.UpdateGame(game.GameId, game);
    }

    public void UpdateGameWithTags(Game game, IList<Tag> selectedTags)
    {
        game.PublisherIdentifier = this.User.UserId;
        this.GameRepository.UpdateGame(game.GameId, game);
        try
        {
            this.DeleteGameTags(game.GameId);
            if (selectedTags != null && selectedTags.Count > EmptyListLength)
            {
                foreach (var tag in selectedTags)
                {
                    this.InsertGameTag(game.GameId, tag.TagId);
                }
            }
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }

    public void DeleteGame(int game_id)
    {
        try
        {
            this.GameRepository.DeleteGame(game_id);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }

    public List<Game> GetDeveloperGames()
    {
        return this.GameRepository.GetDeveloperGames(this.User.UserId);
    }

    public List<Game> GetUnvalidated()
    {
        return this.GameRepository.GetUnvalidated(this.User.UserId);
    }

    public void RejectGame(int gameId)
    {
        this.GameRepository.RejectGame(gameId);
    }

    public void RejectGameWithMessage(int gameId, string message)
    {
        this.GameRepository.RejectGameWithMessage(gameId, message);
    }

    public string GetRejectionMessage(int gameId)
    {
        return this.GameRepository.GetRejectionMessage(gameId);
    }

    public void InsertGameTag(int gameId, int tagId)
    {
        this.GameRepository.InsertGameTag(gameId, tagId);
    }

    public Collection<Tag> GetAllTags()
    {
        return this.TagRepository.GetAllTags();
    }

    public bool IsGameIdInUse(int gameId)
    {
        return this.GameRepository.IsGameIdInUse(gameId);
    }

    public List<Tag> GetGameTags(int gameId)
    {
        return this.GameRepository.GetGameTags(gameId);
    }

    public void DeleteGameTags(int gameId)
    {
        this.GameRepository.DeleteGameTags(gameId);
    }

    public int GetGameOwnerCount(int gameId)
    {
        return this.UserGameRepository.GetGameOwnerCount(gameId);
    }

    public User GetCurrentUser()
    {
        return this.User;
    }

    public Game CreateValidatedGame(string gameIdText, string name, string priceText, string description, string imageUrl, string trailerUrl, string gameplayUrl, string minimumRequirement, string reccommendedRequirement, string discountText, IList<Tag> selectedTags)
    {
        var game = this.ValidateInputForAddingAGame(gameIdText, name, priceText, description, imageUrl, trailerUrl, gameplayUrl, minimumRequirement, reccommendedRequirement, discountText, selectedTags);

        if (this.IsGameIdInUse(game.GameId))
        {
            throw new Exception(ExceptionMessages.IdAlreadyInUse);
        }

        this.CreateGameWithTags(game, selectedTags);
        return game;
    }

    public void DeleteGame(int gameId, ObservableCollection<Game> developerGames)
    {
        // Find and remove the game manually (no lambda)
        Game gameToRemove = null;
        foreach (var game in developerGames)
        {
            if (game.GameId == gameId)
            {
                gameToRemove = game;
                break;
            }
        }

        if (gameToRemove != null)
        {
            developerGames.Remove(gameToRemove);
        }

        // Perform the actual deletion logic (e.g. from DB)
        this.DeleteGame(gameId);
    }

    public void UpdateGameAndRefreshList(Game game, ObservableCollection<Game> developerGames)
    {
        Game existing = null;
        foreach (var gameInDeveloperGames in developerGames)
        {
            if (gameInDeveloperGames.GameId == game.GameId)
            {
                existing = gameInDeveloperGames;
                break;
            }
        }

        if (existing != null)
        {
            developerGames.Remove(existing);
        }

        this.UpdateGame(game); // this should be your existing logic that updates in DB/repo
        developerGames.Add(game);
    }

    public void RejectGameAndRemoveFromUnvalidated(int gameId, ObservableCollection<Game> unvalidatedGames)
    {
        this.RejectGame(gameId);

        Game gameToRemove = null;
        foreach (var game in unvalidatedGames)
        {
            if (game.GameId == gameId)
            {
                gameToRemove = game;
                break;
            }
        }

        if (gameToRemove != null)
        {
            unvalidatedGames.Remove(gameToRemove);
        }
    }

    public bool IsGameIdInUse(int gameId, ObservableCollection<Game> developerGames, ObservableCollection<Game> unvalidatedGames)
    {
        foreach (var game in developerGames)
        {
            if (game.GameId == gameId)
            {
                return true;
            }
        }

        foreach (var game in unvalidatedGames)
        {
            if (game.GameId == gameId)
            {
                return true;
            }
        }

        return this.IsGameIdInUse(gameId); // Call the existing database check or logic
    }

    public IList<Tag> GetMatchingTagsForGame(int gameId, IList<Tag> allAvailableTags)
    {
        List<Tag> matchedTags = new List<Tag>();
        IList<Tag> gameTags = this.GetGameTags(gameId); // Assuming this is already implemented

        foreach (Tag tag in allAvailableTags)
        {
            foreach (Tag gameTag in gameTags)
            {
                if (tag.TagId == gameTag.TagId)
                {
                    matchedTags.Add(tag);
                    break;
                }
            }
        }

        return matchedTags;
    }
}
