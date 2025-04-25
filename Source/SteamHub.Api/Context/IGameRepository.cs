using SteamHub.Api.Entities;

namespace SteamHub.Api.Context;

public interface IGameRepository
{
    Task<Game> CreateGameAsync(Game game);
    Task<Game?> GetGameByIdAsync(int id);
    Task SaveChangesAsync();
    Task<List<Game>> GetGamesAsync(GamesQueryParams queryParams);
    Task<Game> UpdateGameAsync(Game game);
    Task DeleteGameAsync(int id);
    Task InsertGameTag(int gameId, params int[] tagIds);
    Task DeleteGameTag(int gameId, params int[] tagIds);
}