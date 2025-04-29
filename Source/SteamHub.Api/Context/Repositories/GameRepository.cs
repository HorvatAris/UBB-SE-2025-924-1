using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Context.Repositories;

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
        var game = await context.Games
            .Include(g => g.Tags)
            .Include(g => g.Publisher)
            .Include(g => g.Status)
            .FirstOrDefaultAsync(g => g.GameId == id);

        return game == null ? null : MapToGameDetailedResponse(game);
    }

    public Task<List<GameDetailedResponse>> GetGamesAsync(GetGamesRequest parameters)
    {
        IQueryable<Game> query = context.Games;
        if (parameters.StatusIs != null)
        {
            query = query.Where(g => g.StatusId == parameters.StatusIs);
        }

        if (parameters.PublisherIdentifierIs != null)
        {
            query = query.Where(g => g.Publisher.UserId == parameters.PublisherIdentifierIs);
        }

        if (parameters.PublisherIdentifierIsnt != null)
        {
            query = query.Where(g => g.Publisher.UserId != parameters.PublisherIdentifierIsnt);
        }

        return query
            .Include(g => g.Tags)
            .Include(g => g.Publisher)
            .Include(g => g.Status)
            .Select(game => MapToGameDetailedResponse(game))
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
            .Include(g => g.Publisher)
            .Include(g => g.Tags)
            .Include(g => g.Status)
            .FirstOrDefaultAsync(g => g.GameId == id);
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
        var game = await context.Games
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.GameId == gameId);

        if (game == null)
        {
            throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }

        var tagIdSet = new HashSet<int>(tagIds.Where(tagId => game.Tags.All(tag => tag.TagId != tagId)));

        var tags = await context.Tags.Where(tag => tagIdSet.Contains(tag.TagId)).ToListAsync();

        foreach (var tag in tags)
        {
            game.Tags.Add(tag);
        }

        await SaveChangesAsync();
    }

    private async Task DeleteGameTag(int gameId, params int[] tagIds)
    {
        var game = await context.Games
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.GameId == gameId);
        if (game == null)
        {
            throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }

        if (!tagIds.Any())
        {
            game.Tags.Clear();
        }
        else
        {
            var tagsToRemove = game.Tags.Where(tag => tagIds.Contains(tag.TagId));
            foreach (var tag in tagsToRemove)
            {
                game.Tags.Remove(tag);
            }
        }

        await SaveChangesAsync();
    }
    
    private async Task ReplaceGameTag(int gameId, params int[] tagIds)
    {
        var game = await this.context.Games
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.GameId == gameId);

        if (game == null)
        {
            throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }

        var tagIdSet = new HashSet<int>(tagIds);

        var tags = await this.context.Tags.Where(tag => tagIdSet.Contains(tag.TagId)).ToListAsync();

        game.Tags.Clear();
        foreach (var tag in tags)
        {
            game.Tags.Add(tag);
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
                t => new TagDetailedResponse
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                }).ToList()
        };
    }

    private async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}