// <copyright file="DeveloperService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CtrlAltElite.ServiceProxies;
using CtrlAltElite.Services;
using Refit;
using SteamHub.ApiContract.Models.Game;
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

    public IGameServiceProxy GameServiceProxy { get; set; }

    //public ITagRepository TagRepository { get; set; }
    public ITagServiceProxy TagServiceProxy { get; set; }

    public IUserGameRepository UserGameRepository { get; set; }

    public User User { get; set; }

    public async Task ValidateGame(int game_id)
    {
        await this.GameServiceProxy.UpdateGameAsync(
            game_id,
            new UpdateGameRequest
            {
                Status = GameStatusEnum.Approved
            });
    }

    public Game ValidateInputForAddingAGame(
        string gameIdText,
        string name,
        string priceText,
        string description,
        string imageUrl,
        string trailerUrl,
        string gameplayUrl,
        string minimumRequirement,
        string reccommendedRequirement,
        string discountText,
        IList<Tag> selectedTags)
    {
        if (string.IsNullOrWhiteSpace(gameIdText) || string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(priceText) ||
            string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(imageUrl) ||
            string.IsNullOrWhiteSpace(minimumRequirement) ||
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

        if (!decimal.TryParse(discountText, out var discount) || discount < ComparingValueForMinimumDicount ||
            discount > ComparingValueForMaximumDicount)
        {
            throw new Exception(ExceptionMessages.InvalidDiscount);
        }

        if (selectedTags == null || selectedTags.Count == EmptyListLength)
        {
            throw new Exception(ExceptionMessages.NoTagsSelected);
        }

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

    public async Task CreateGame(Game game)
    {
        game.PublisherIdentifier = this.User.UserId;

        await this.GameServiceProxy.CreateGameAsync(
            new CreateGameRequest
            {
                Description = game.GameDescription,
                ImagePath = game.ImagePath,
                Name = game.GameTitle,
                Price = game.Price,
                RecommendedRequirements = game.RecommendedRequirements,
                MinimumRequirements = game.MinimumRequirements,
                Discount = game.Discount,
                NumberOfRecentPurchases = game.NumberOfRecentPurchases,
                Rating = game.Rating,
                PublisherUserIdentifier = game.PublisherIdentifier,
                TrailerPath = game.TrailerPath,
                GameplayPath = game.GameplayPath,
            });
    }

    public async Task CreateGameWithTags(Game game, IList<Tag> selectedTags)
    {
        await this.CreateGame(game);
        System.Diagnostics.Debug.WriteLine(game.GameId);
        //await Task.Delay(100);
        foreach (var tag in selectedTags)
        {
            System.Diagnostics.Debug.WriteLine("the tag id" + tag.TagId);
        }

        if (selectedTags != null && selectedTags.Count > EmptyListLength)
        {
            foreach (var tag in selectedTags)
            {
                await this.InsertGameTag(game.GameId, tag.TagId);
            }
        }
    }

    public async Task UpdateGame(Game game)
    {
        game.PublisherIdentifier = this.User.UserId;

        await this.GameServiceProxy.UpdateGameAsync(
            game.GameId,
            new UpdateGameRequest
            {
                Description = game.GameDescription,
                ImagePath = game.ImagePath,
                Name = game.GameTitle,
                Price = game.Price,
                RecommendedRequirements = game.RecommendedRequirements,
                MinimumRequirements = game.MinimumRequirements,
                Discount = game.Discount,
                NumberOfRecentPurchases = game.NumberOfRecentPurchases,
                Rating = game.Rating,
                TrailerPath = game.TrailerPath,
                GameplayPath = game.GameplayPath,
            });
    }

    public async Task UpdateGameWithTags(Game game, IList<Tag> selectedTags)
    {
        game.PublisherIdentifier = this.User.UserId;
        await this.GameServiceProxy.UpdateGameAsync(
            game.GameId,
            new UpdateGameRequest
            {
                Description = game.GameDescription,
                ImagePath = game.ImagePath,
                Name = game.GameTitle,
                Price = game.Price,
                RecommendedRequirements = game.RecommendedRequirements,
                MinimumRequirements = game.MinimumRequirements,
                Discount = game.Discount,
                NumberOfRecentPurchases = game.NumberOfRecentPurchases,
                Rating = game.Rating,
                TrailerPath = game.TrailerPath,
                GameplayPath = game.GameplayPath,
            });

        await this.GameServiceProxy.PatchGameTagsAsync(
            game.GameId,
            new PatchGameTagsRequest
            {
                TagIds = new HashSet<int>(selectedTags.Select(tag => tag.TagId)),
                Type = GameTagsPatchType.Replace,
            });
    }

    public async Task DeleteGame(int game_id)
    {
        await this.GameServiceProxy.DeleteGameAsync(game_id);
    }

    public async Task<List<Game>> GetDeveloperGames()
    {
        var games = await this.GameServiceProxy.GetGamesAsync(
            new GetGamesRequest
            {
                PublisherIdentifierIs = this.User.UserId,
            });
        return games.Select(GameMapper.MapToGame).ToList();
    }

    public async Task<List<Game>> GetUnvalidated()
    {
        var games = await this.GameServiceProxy.GetGamesAsync(
            new GetGamesRequest
            {
                StatusIs = GameStatusEnum.Pending,
                PublisherIdentifierIsnt = this.User.UserId,
            });
        return games.Select(GameMapper.MapToGame).ToList();
    }

    public async Task RejectGame(int gameId)
    {
        await this.GameServiceProxy.UpdateGameAsync(
            gameId,
            new UpdateGameRequest
            {
                Status = GameStatusEnum.Rejected,
            });
    }

    public async Task RejectGameWithMessage(int gameId, string message)
    {
        await this.GameServiceProxy.UpdateGameAsync(
            gameId,
            new UpdateGameRequest
            {
                Status = GameStatusEnum.Rejected,
                RejectMessage = message,
            });
    }

    public async Task<string> GetRejectionMessage(int gameId)
    {
        return (await this.GameServiceProxy.GetGameByIdAsync(gameId)) !.RejectMessage;
    }

    public async Task InsertGameTag(int gameId, int tagId)
    {
        System.Diagnostics.Debug.WriteLine($"InsertGameTag called with GameId: {gameId}, TagId: {tagId}");

        try
        {
            try
            {
                var game = await this.GameServiceProxy.GetGameByIdAsync(gameId);
            }
            catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new Exception($"Game with ID {gameId} not found.");
        }

            await this.GameServiceProxy.PatchGameTagsAsync(
            gameId,
            new PatchGameTagsRequest
                    {
                        TagIds = new HashSet<int> { tagId }, // Ensure tagId is wrapped in a collection
                        Type = GameTagsPatchType.Insert,
                    });

            // Log success
            System.Diagnostics.Debug.WriteLine($"Successfully inserted TagId: {tagId} for GameId: {gameId}");
        }
        catch (Exception ex)
        {
            // Log the exception
            System.Diagnostics.Debug.WriteLine($"Error in InsertGameTag: {ex.Message}");
            throw; // Re-throw the exception for further handling
        }
    }

    public async Task<Collection<Tag>> GetAllTags()
    {
        var tagsResponse = await this.TagServiceProxy.GetAllTagsAsync();
        return new Collection<Tag>(
                    tagsResponse.Tags.Select(TagMapper.MapToTag).ToList()
                );
    }

    public async Task<List<Tag>> GetGameTags(int gameId)
    {
        var game = (await this.GameServiceProxy.GetGameByIdAsync(gameId)) !;
        return game.Tags.Select(
            tag => new Tag
            {
                TagId = tag.TagId,
                Tag_name = tag.TagName,
                NumberOfUserGamesWithTag = 0,
            }).ToList();
    }

    public async Task<bool> IsGameIdInUse(int gameId)
    {
        try
        {
            await this.GameServiceProxy.GetGameByIdAsync(gameId);
            return true;
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task DeleteGameTags(int gameId)
    {
        await this.GameServiceProxy.PatchGameTagsAsync(
            gameId,
            new PatchGameTagsRequest
            {
                TagIds = new HashSet<int>(),
                Type = GameTagsPatchType.Delete,
            });
    }

    public int GetGameOwnerCount(int gameId)
    {
        return this.UserGameRepository.GetGameOwnerCount(gameId);
    }

    public User GetCurrentUser()
    {
        return this.User;
    }

    public async Task<Game> CreateValidatedGame(
        string gameIdText,
        string name,
        string priceText,
        string description,
        string imageUrl,
        string trailerUrl,
        string gameplayUrl,
        string minimumRequirement,
        string reccommendedRequirement,
        string discountText,
        IList<Tag> selectedTags)
    {
        var game = this.ValidateInputForAddingAGame(
            gameIdText,
            name,
            priceText,
            description,
            imageUrl,
            trailerUrl,
            gameplayUrl,
            minimumRequirement,
            reccommendedRequirement,
            discountText,
            selectedTags);

        if (await this.IsGameIdInUse(game.GameId))
        {
            throw new Exception(ExceptionMessages.IdAlreadyInUse);
        }

        await this.CreateGameWithTags(game, selectedTags);
        return game;
    }

    public async Task DeleteGame(int gameId, ObservableCollection<Game> developerGames)
    {
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
        await this.DeleteGame(gameId);
    }

    public async Task UpdateGameAndRefreshList(Game game, ObservableCollection<Game> developerGames)
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

        await this.UpdateGame(game); // this should be your existing logic that updates in DB/repo
        developerGames.Add(game);
    }

    public async Task RejectGameAndRemoveFromUnvalidated(int gameId, ObservableCollection<Game> unvalidatedGames)
    {
        await this.RejectGame(gameId);

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

    public async Task<bool> IsGameIdInUse(
        int gameId,
        ObservableCollection<Game> developerGames,
        ObservableCollection<Game> unvalidatedGames)
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

        return await this.IsGameIdInUse(gameId); // Call the existing database check or logic
    }

    public async Task<IList<Tag>> GetMatchingTagsForGame(int gameId, IList<Tag> allAvailableTags)
    {
        List<Tag> matchedTags = new List<Tag>();
        IList<Tag> gameTags = await this.GetGameTags(gameId); // Assuming this is already implemented

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