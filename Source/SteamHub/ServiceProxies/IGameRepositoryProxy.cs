// <copyright file="IGameServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ServiceProxies
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Refit;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Repositories;

    public interface IGameRepositoryProxy : IGameRepository
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