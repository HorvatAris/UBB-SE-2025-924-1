// <copyright file="DeveloperService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SteamHub.ApiContract.Constants;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Proxies;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using static SteamHub.ApiContract.Constants.NotificationStrings;

public class DeveloperService : IDeveloperService
{
    private const int ComparingValueForPositivePrice = 0;
    private const int ComparingValueForMinimumDicount = 0;
    private const int ComparingValueForMaximumDicount = 100;
    private const int EmptyListLength = 0;
    private const string PendingState = "Pending";

    public DeveloperService(IGameRepository gameRepository, ITagRepository tagRepository, IUsersGamesRepository userGameRepository, IUserRepository userRepository, IItemRepository itemRepository, IItemTradeDetailRepository itemTradeDetailRepository, 
        IUserDetails user)
    {
        this.GameRepository = gameRepository;
        this.TagRepository = tagRepository;
        this.UserGameRepository = userGameRepository;
        this.UserRepository = userRepository;
        this.ItemRepository = itemRepository;
        this.ItemTradeDetailRepository = itemTradeDetailRepository;
        this.User = user;
    }

    public IGameRepository GameRepository { get; set; }

    public ITagRepository TagRepository { get; set; }

    public IUsersGamesRepository UserGameRepository { get; set; }

    public IUserRepository UserRepository { get; set; }

    public IItemRepository ItemRepository { get; set; }

    public IItemTradeDetailRepository ItemTradeDetailRepository { get; set; }

    public IUserDetails User { get; set; }

    public async Task ValidateGameAsync(int game_id)
    {
        await this.GameRepository.UpdateGameAsync(
            game_id,
            new UpdateGameRequest
            {
                Status = GameStatusEnum.Approved,
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
            PublisherIdentifier = this.User.UserId,
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

    public async Task CreateGameAsync(Game game)
    {
        game.PublisherIdentifier = this.User.UserId;

        await this.GameRepository.CreateGameAsync(
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

    public async Task CreateGameWithTagsAsync(Game game, IList<Tag> selectedTags)
    {
        await this.CreateGameAsync(game);

        // if (selectedTags != null && selectedTags.Count > EmptyListLength)
        // {
        //    foreach (var tag in selectedTags)
        //    {
        //        try
        //        {
        //            await this.InsertGameTag(game.GameId, tag.TagId);
        //        }
        //        catch (Exception exception)
        //        {
        //            System.Diagnostics.Debug.WriteLine(exception.Message);
        //            continue;
        //        }
        //    }
        // }
    }

    public async Task UpdateGameAsync(Game game)
    {
        game.PublisherIdentifier = this.User.UserId;

        await this.GameRepository.UpdateGameAsync(
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

    public async Task UpdateGameWithTagsAsync(Game game, IList<Tag> selectedTags)
    {
        game.PublisherIdentifier = this.User.UserId;
        await this.GameRepository.UpdateGameAsync(
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

        await this.GameRepository.PatchGameTagsAsync(
            game.GameId,
            new PatchGameTagsRequest
            {
                TagIds = new HashSet<int>(selectedTags.Select(tag => tag.TagId)),
                Type = GameTagsPatchType.Replace,
            });
    }

    public async Task DeleteGameAsync(int game_id)
    {
        var allItemsFromThisGame = await this.ItemRepository.GetItemsAsync();
        List<int> allItemsFromThisGameIds = new List<int>();
        foreach (var item in allItemsFromThisGame)
        {
            if (item.GameId == game_id)
            {
                allItemsFromThisGameIds.Add(item.ItemId);
            }
        }

        System.Diagnostics.Debug.WriteLine(allItemsFromThisGameIds.Count);

        var itemTradeDetails = await this.ItemTradeDetailRepository.GetItemTradeDetailsAsync();
        List<(int, int)> tradeItemPairs = new List<(int, int)>();
        foreach (var itemId in allItemsFromThisGameIds)
        {
            foreach (var itemTradeDetail in itemTradeDetails.ItemTradeDetails)
            {
                if (itemId == itemTradeDetail.ItemId)
                {
                    tradeItemPairs.Add((itemTradeDetail.TradeId, itemId));
                }
            }
        }

        foreach (var pair in tradeItemPairs)
        {
            try
            {
                await this.ItemTradeDetailRepository.DeleteItemTradeDetailAsync(pair.Item1, pair.Item2);
            }
            catch (Exception)
            {
                continue;
            }
        }

        System.Diagnostics.Debug.WriteLine(game_id);
        await this.GameRepository.DeleteGameAsync(game_id);
    }

    public async Task<List<Game>> GetDeveloperGamesAsync()
    {
        var games = await this.GameRepository.GetGamesAsync(
            new GetGamesRequest
            {
                PublisherIdentifierIs = this.User.UserId,
            });
        return games.Select(GameMapper.MapToGame).ToList();
    }

    public async Task<List<Game>> GetUnvalidatedAsync()
    {
        var games = await this.GameRepository.GetGamesAsync(
            new GetGamesRequest
            {
                StatusIs = GameStatusEnum.Pending,
                PublisherIdentifierIsnt = this.User.UserId,
            });
        return games.Select(GameMapper.MapToGame).ToList();
    }

    public async Task RejectGameAsync(int gameId)
    {
        await this.GameRepository.UpdateGameAsync(
            gameId,
            new UpdateGameRequest
            {
                Status = GameStatusEnum.Rejected,
            });
    }

    public async Task RejectGameWithMessageAsync(int gameId, string message)
    {
        await this.GameRepository.UpdateGameAsync(
            gameId,
            new UpdateGameRequest
            {
                Status = GameStatusEnum.Rejected,
                RejectMessage = message,
            });
    }

    public async Task<string> GetRejectionMessageAsync(int gameId)
    {
        return (await this.GameRepository.GetGameByIdAsync(gameId)) !.RejectMessage;
    }

    public async Task InsertGameTagAsync(int gameId, int tagId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine(gameId + tagId);
            await this.GameRepository.PatchGameTagsAsync(
                gameId,
                new PatchGameTagsRequest
                {
                    TagIds = new HashSet<int>(tagId),
                    Type = GameTagsPatchType.Insert,
                });
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception.Message);
            throw;
        }
    }

    public async Task<Collection<Tag>> GetAllTagsAsync()
    {
        var tagsResponse = await this.TagRepository.GetAllTagsAsync();
        return new Collection<Tag>(
                    tagsResponse.Tags.Select(TagMapper.MapToTag).ToList());
    }

    public async Task<List<Tag>> GetGameTagsAsync(int gameId)
    {
        try
        {
            var game = (await this.GameRepository.GetGameByIdAsync(gameId))!;
            return game.Tags.Select(
                tag => new Tag
                {
                    TagId = tag.TagId,
                    Tag_name = tag.TagName,
                    NumberOfUserGamesWithTag = 0,
                }).ToList();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new List<Tag>();
        }
    }

    public async Task<bool> IsGameIdInUseAsync(int gameId)
    {
        try
        {
            var game = await this.GameRepository.GetGameByIdAsync(gameId);
            return game != null; 
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false; 
        }
    }

    public async Task DeleteGameTagsAsync(int gameId)
    {
        await this.GameRepository.PatchGameTagsAsync(
            gameId,
            new PatchGameTagsRequest
            {
                TagIds = new HashSet<int>(),
                Type = GameTagsPatchType.Delete,
            });
    }

    public async Task<int> GetGameOwnerCountAsync(int gameId)
    {
        var allUsers = await this.UserRepository.GetUsersAsync();
        int ownedCount = 0;
        System.Diagnostics.Debug.WriteLine(allUsers.Users.Count);
        foreach (var userResponse in allUsers.Users)
        {
            int userId = userResponse.UserId;
            try
            {
                var games = await this.UserGameRepository.GetUserGamesAsync(userId);

                foreach (var game in games.UserGames)
                {
                    if (game.GameId == gameId)
                    {
                        ownedCount++;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                continue;
            }
        }

        System.Diagnostics.Debug.WriteLine(ownedCount);
        return ownedCount;
    }

    public async Task<Game> CreateValidatedGameAsync(
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

        if (await this.IsGameIdInUseAsync(game.GameId))
        {
            throw new Exception(ExceptionMessages.IdAlreadyInUse);
        }

        await this.CreateGameWithTagsAsync(game, selectedTags);
        return game;
    }

    public async Task DeleteGameAsync(int gameId, ObservableCollection<Game> developerGames)
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

        // Perform the actual deletion logic
        await this.DeleteGameAsync(gameId);
    }

    public async Task UpdateGameAndRefreshListAsync(Game game, ObservableCollection<Game> developerGames)
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

        await this.UpdateGameAsync(game); 
        developerGames.Add(game);
    }

    public async Task RejectGameAndRemoveFromUnvalidatedAsync(int gameId, ObservableCollection<Game> unvalidatedGames)
    {
        await this.RejectGameAsync(gameId);

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

    public async Task<bool> IsGameIdInUseAsync(
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

        return await this.IsGameIdInUseAsync(gameId); 
    }

    public async Task<IList<Tag>> GetMatchingTagsForGameAsync(int gameId, IList<Tag> allAvailableTags)
    {
        List<Tag> matchedTags = new List<Tag>();
        List<Tag> gameTags = await this.GetGameTagsAsync(gameId);

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