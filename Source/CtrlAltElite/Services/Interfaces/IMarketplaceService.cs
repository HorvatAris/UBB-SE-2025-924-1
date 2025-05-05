using CtrlAltElite.Models;
using SteamStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CtrlAltElite.Services.Interfaces
{
    public interface IMarketplaceService
    {
        User User { get; set; }

        Task AddListingAsync(Game game, Item item);

        Task<bool> BuyItemAsync(Item item, int userId);

        Task<List<Item>> GetAllListingsAsync();

        Task<List<User>> GetAllUsersAsync();

        Task<List<Item>> GetListingsByGameAsync(Game game, int userId);

        Task RemoveListingAsync(Game game, Item item);

        Task UpdateListingAsync(Game game, Item item);
    }
}