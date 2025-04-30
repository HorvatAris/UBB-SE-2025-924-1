using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SteamHub.ApiContract.Models.Game;

namespace CtrlAltElite.ServiceProxies
{
    using SteamHub.ApiContract.Repositories;

    public interface IGameServiceProxy : IGameRepository
    {
        [Post("/api/Games")]
        Task<GameDetailedResponse> CreateGameAsync([Body] CreateGameRequest game);

        [Get("/api/Games/{id}")]
        Task<GameDetailedResponse?> GetGameByIdAsync(int id);

        [Get("/api/Games")]
        Task<List<GameDetailedResponse>> GetGamesAsync([Query] GetGamesRequest request);

        [Patch("/api/Games/{id}")]
        Task UpdateGameAsync(int id, [Body] UpdateGameRequest game);

        [Delete("/api/Games/{id}")]
        Task DeleteGameAsync(int id);

        [Patch("/api/Games/{id}/tags")]
        Task PatchGameTagsAsync(int id, [Body] PatchGameTagsRequest tags);
    }
}