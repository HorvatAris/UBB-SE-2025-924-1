namespace SteamHub.Api.Context;

using Models;

public interface IGameRepository
{
    Task<GameDetailedResponse> CreateGameAsync(CreateGameRequest game);
    Task<GameDetailedResponse?> GetGameByIdAsync(int id);
    Task<List<GameDetailedResponse>> GetGamesAsync(GetGamesRequest request);
    Task<GameDetailedResponse> UpdateGameAsync(int id, UpdateGameRequest game);
    Task DeleteGameAsync(int id);
    Task InsertGameTag(int gameId, params int[] tagIds);
    Task DeleteGameTag(int gameId, params int[] tagIds);
}