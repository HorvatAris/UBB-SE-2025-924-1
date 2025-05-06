// <copyright file="ITagServiceProxy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ServiceProxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Refit;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Repositories;

    public interface ITagServiceProxy : ITagRepository
    {
        [Get("/api/Tags")]
        Task<GetTagsResponse> GetAllTagsAsync();
    }
}
