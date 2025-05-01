using Refit;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.ApiContract.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.ServiceProxies
{
    public interface ITagServiceProxy : ITagRepository
    {
        [Get("/api/Tags")]
        Task<GetTagsResponse> GetAllTagsAsync();
    }
}
