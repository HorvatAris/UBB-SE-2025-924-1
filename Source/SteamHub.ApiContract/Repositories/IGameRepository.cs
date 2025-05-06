namespace SteamHub.ApiContract.Repositories
{
    using Models.Game;

    public interface IGameRepository
    {
        Task<GameDetailedResponse> CreateGameAsync(CreateGameRequest game);

        Task<GameDetailedResponse?> GetGameByIdAsync(int id);

        Task<List<GameDetailedResponse>> GetGamesAsync(GetGamesRequest request);

        Task UpdateGameAsync(int id, UpdateGameRequest game);

        Task DeleteGameAsync(int id);

        Task PatchGameTagsAsync(int id, PatchGameTagsRequest tags);
    }
}