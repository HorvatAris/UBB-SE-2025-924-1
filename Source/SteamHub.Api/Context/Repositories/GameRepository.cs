using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Context.Repositories;

using Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;

public class GameRepository : IGameRepository
{
    private readonly DataContext context;

    // Inject the DataContext
    public GameRepository(DataContext context)
    {
        this.context = context;
    }

    // Method to create a new game
    public async Task<GameDetailedResponse> CreateGameAsync(CreateGameRequest createRequest)
    {
        var publisherUser = await context.Users.FindAsync(createRequest.PublisherUserIdentifier);

        if (publisherUser == null)
        {
            throw new ArgumentException($"User with id {createRequest.PublisherUserIdentifier} was not found");
        }

        var entity = new Game
        {
            Name = createRequest.Name,
            Description = createRequest.Description,
            Price = createRequest.Price,
            MinimumRequirements = createRequest.MinimumRequirements,
            RecommendedRequirements = createRequest.RecommendedRequirements,
            StatusId = GameStatusEnum.Pending,
            NumberOfRecentPurchases = createRequest.NumberOfRecentPurchases,
            Discount = createRequest.Discount,
            GameplayPath = createRequest.GameplayPath,
            TrailerPath = createRequest.TrailerPath,
            ImagePath = createRequest.ImagePath,
            Rating = createRequest.Rating,
            Publisher = publisherUser,
        };
        context.Games.Add(entity);

        await SaveChangesAsync();

        return MapToGameDetailedResponse(entity);
    }

    public async Task<GameDetailedResponse?> GetGameByIdAsync(int id)
    {
        var currentGame = await context.Games
            .Include(game => game.Tags)
            .Include(game => game.Publisher)
            .Include(game => game.Status)
            .FirstOrDefaultAsync(game => game.GameId == id);

        return currentGame == null ? null : MapToGameDetailedResponse(currentGame);
    }

    public Task<List<GameDetailedResponse>> GetGamesAsync(GetGamesRequest parameters)
    {
        IQueryable<Game> query = context.Games;
        if (parameters.StatusIs != null)
        {
            query = query.Where(game => game.StatusId == parameters.StatusIs);
        }

        if (parameters.PublisherIdentifierIs != null)
        {
            query = query.Where(game => game.Publisher.UserId == parameters.PublisherIdentifierIs);
        }

        if (parameters.PublisherIdentifierIsnt != null)
        {
            query = query.Where(game => game.Publisher.UserId != parameters.PublisherIdentifierIsnt);
        }

        return query
            .Include(game => game.Tags)
            .Include(game => game.Publisher)
            .Include(game => game.Status)
            .Select(currentGame => MapToGameDetailedResponse(currentGame))
            .ToListAsync();
    }

    public async Task DeleteGameAsync(int id)
    {
        var game = await context.Games.FindAsync(id);

        if (game == null)
        {
            throw new ArgumentException($"Game with id {id} was not found");
        }

        context.Games.Remove(game);

        await context.SaveChangesAsync();
    }

    public async Task UpdateGameAsync(int id, UpdateGameRequest request)
    {
        var existingGame = await context.Games
            .Include(game => game.Publisher)
            .Include(game => game.Tags)
            .Include(game => game.Status)
            .FirstOrDefaultAsync(game => game.GameId == id);
        if (existingGame == null)
        {
            throw new KeyNotFoundException($"Game with ID {id} not found.");
        }

        if (request.Name != null)
        {
            existingGame.Name = request.Name;
        }

        if (request.Description != null)
        {
            existingGame.Description = request.Description;
        }

        if (request.Price != null)
        {
            existingGame.Price = (decimal)request.Price;
        }

        if (request.MinimumRequirements != null)
        {
            existingGame.MinimumRequirements = request.MinimumRequirements;
        }

        if (request.RecommendedRequirements != null)
        {
            existingGame.RecommendedRequirements = request.RecommendedRequirements;
        }

        if (request.Status != null)
        {
            existingGame.StatusId = (GameStatusEnum)request.Status;
        }

        if (request.Rating != null)
        {
            existingGame.Rating = (decimal)request.Rating;
        }

        if (request.NumberOfRecentPurchases != null)
        {
            existingGame.NumberOfRecentPurchases = (int)request.NumberOfRecentPurchases;
        }

        if (request.TrailerPath != null)
        {
            existingGame.TrailerPath = request.TrailerPath;
        }

        if (request.GameplayPath != null)
        {
            existingGame.GameplayPath = request.GameplayPath;
        }

        if (request.Discount != null)
        {
            existingGame.Discount = (decimal)request.Discount;
        }

        if (request.RejectMessage != null)
        {
            existingGame.RejectMessage = request.RejectMessage;
        }

        await SaveChangesAsync();
    }
    
    public async Task PatchGameTagsAsync(int id, PatchGameTagsRequest tags)
    {
       
        if (tags.Type == GameTagsPatchType.Insert)
        {
            await InsertGameTag(id, tags.TagIds.ToArray());
        }
        else if(tags.Type == GameTagsPatchType.Delete)
        {
            await DeleteGameTag(id, tags.TagIds.ToArray());
        }
        else
        {
            await ReplaceGameTag(id, tags.TagIds.ToArray());
        }
    }

    private async Task InsertGameTag(int gameId, params int[] tagIds)
    {
        var currentGame = await context.Games
            .Include(game => game.Tags)
            .FirstOrDefaultAsync(game => game.GameId == gameId);

        if (currentGame == null)
        {
            throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }

        var tagIdSet = new HashSet<int>(tagIds.Where(tagId => currentGame.Tags.All(tag => tag.TagId != tagId)));

        var tags = await context.Tags.Where(tag => tagIdSet.Contains(tag.TagId)).ToListAsync();

        foreach (var tag in tags)
        {
            currentGame.Tags.Add(tag);
        }

        await SaveChangesAsync();
    }

    private async Task DeleteGameTag(int gameId, params int[] tagIds)
    {
        var currentGame = await context.Games
            .Include(game => game.Tags)
            .FirstOrDefaultAsync(game => game.GameId == gameId);
        if (currentGame == null)
        {
            throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }

        if (!tagIds.Any())
        {
            currentGame.Tags.Clear();
        }
        else
        {
            var tagsToRemove = currentGame.Tags.Where(tag => tagIds.Contains(tag.TagId));
            foreach (var tag in tagsToRemove)
            {
                currentGame.Tags.Remove(tag);
            }
        }

        await SaveChangesAsync();
    }
    
    private async Task ReplaceGameTag(int gameId, params int[] tagIds)
    {
        var currentGame = await this.context.Games
            .Include(game => game.Tags)
            .FirstOrDefaultAsync(game => game.GameId == gameId);

        if (currentGame == null)
        {
            throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }

        var tagIdSet = new HashSet<int>(tagIds);

        var tags = await this.context.Tags.Where(tag => tagIdSet.Contains(tag.TagId)).ToListAsync();

        currentGame.Tags.Clear();
        foreach (var tag in tags)
        {
            currentGame.Tags.Add(tag);
        }

        await SaveChangesAsync();
    }



    private static GameDetailedResponse MapToGameDetailedResponse(Game entity)
    {
        return new GameDetailedResponse
        {
            Identifier = entity.GameId,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            MinimumRequirements = entity.MinimumRequirements,
            RecommendedRequirements = entity.RecommendedRequirements,
            Status = entity.StatusId,
            NumberOfRecentPurchases = entity.NumberOfRecentPurchases,
            Discount = entity.Discount,
            GameplayPath = entity.GameplayPath,
            TrailerPath = entity.TrailerPath,
            ImagePath = entity.ImagePath,
            Rating = entity.Rating,
            RejectMessage = entity.RejectMessage,
            PublisherUserIdentifier = entity.Publisher.UserId,
            Tags = entity.Tags.Select(
                tag => new TagDetailedResponse
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName,
                }).ToList()
        };
    }

    private async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}