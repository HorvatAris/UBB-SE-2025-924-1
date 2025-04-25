using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;

namespace SteamHub.Api.Context;

public class GameRepository : IGameRepository
{
    private readonly DataContext _context;

    // Inject the DataContext
    public GameRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Game> UpdateGameAsync(Game game)
    {
        
        var existingGame = await _context.Games
            .FirstOrDefaultAsync(g => g.Identifier == game.Identifier);
        if (existingGame == null)
        {
            throw new KeyNotFoundException($"Game with ID {game.Identifier} not found.");
        }
        existingGame.Name = game.Name;
        existingGame.Description = game.Description;
        existingGame.Price = game.Price;
        existingGame.MinimumRequirements = game.MinimumRequirements;
        existingGame.RecommendedRequirements = game.RecommendedRequirements;
        existingGame.Status = game.Status;
        existingGame.Rating = game.Rating;
        existingGame.NumberOfRecentPurchases = game.NumberOfRecentPurchases;
        existingGame.TrendingScore = game.TrendingScore;
        existingGame.TrailerPath = game.TrailerPath;
        existingGame.GameplayPath = game.GameplayPath;
        existingGame.Discount = game.Discount;
        existingGame.TagScore = game.TagScore;
        existingGame.PublisherIdentifier = game.PublisherIdentifier;
        
   
        
        await SaveChangesAsync();

        return existingGame;

    }

    // Method to create a new game
    public async Task<Game> CreateGameAsync(Game game)
    {
        _context.Games.Add(game);

        // Save changes to the database
        await SaveChangesAsync();

        return game;
    }

    // Method to get a game by its ID
    public async Task<Game?> GetGameByIdAsync(int id)
    {
        return await _context.Games
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.Identifier == id);
    }

    // Method to save changes to the database
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task<List<Game>> GetGamesAsync(GamesQueryParams parameters)
    {
        IQueryable<Game> query = _context.Games;
        if (parameters.StatusIs != null)
        {
            query= query.Where(g => g.Status == parameters.StatusIs);
        }

        if (parameters.PublisherIdentifierIs != null)
        {
            query = query.Where(g => g.PublisherIdentifier == parameters.PublisherIdentifierIs);
        }
        if (parameters.PublisherIdentifierIsnt != null)
        {
            query = query.Where(g => g.PublisherIdentifier != parameters.PublisherIdentifierIs);
        }
        return query
            .Include(g => g.Tags)
            .ToListAsync();
    }
    

    public async Task DeleteGameAsync(int id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
        {
            throw new KeyNotFoundException($"Game with ID {id} not found.");
        }

        // Remove the game entity
        _context.Games.Remove(game);

        // Save changes to the database
        await _context.SaveChangesAsync();
    }

    public async Task InsertGameTag(int gameId, params int[] tagIds)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }
        var tagIdSet = new HashSet<int>(
            tagIds.Where(tagId => game.Tags.All(tag => tag.TagId != tagId))
        );

        var tags = await _context.Tags.Where(tag => tagIdSet.Contains(tag.TagId)).ToListAsync();

        foreach (var tag in tags)
        {
            game.Tags.Add(tag);
        }

        await SaveChangesAsync();
    }

    public async Task DeleteGameTag(int gameId, params int[] tagIds)
    {
        var game = await GetGameByIdAsync(gameId);
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
}