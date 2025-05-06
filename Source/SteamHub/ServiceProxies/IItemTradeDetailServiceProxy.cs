// <copyright file="IItemTradeDetailServiceProxy.cs" company="PlaceholderCompany">
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
    using SteamHub.ApiContract.Models.ItemTradeDetails;

    public interface IItemTradeDetailServiceProxy
    {
        [Get("/api/ItemTradeDetails")]
        Task<GetItemTradeDetailsResponse> GetAllItemTradeDetailsAsync();

        [Get("/api/ItemTradeDetails/{tradeId}/{itemId}")]
        Task<ItemTradeDetailResponse?> GetItemTradeDetailAsync(int tradeId, int itemId);

        [Post("/api/ItemTradeDetails")]
        Task<CreateItemTradeDetailResponse> CreateItemTradeDetailAsync([Body] CreateItemTradeDetailRequest request);

        [Delete("/api/ItemTradeDetails/{tradeId}/{itemId}")]
        Task DeleteItemTradeDetailAsync(int tradeId, int itemId);
    }
}
