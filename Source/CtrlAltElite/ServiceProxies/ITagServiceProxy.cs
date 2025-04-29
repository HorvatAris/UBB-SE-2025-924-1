using Refit;
using SteamStore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Models.Tag;
namespace CtrlAltElite.ServiceProxies
{
    public interface ITagServiceProxy : ITagRepository
    {
        [Get("api/Tags")]
        Task<List<TagDetailedResponse>> GetAllTagsAsync();

    }
}
