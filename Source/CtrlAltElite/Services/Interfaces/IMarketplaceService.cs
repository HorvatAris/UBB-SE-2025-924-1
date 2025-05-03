using CtrlAltElite.Models;
using SteamStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CtrlAltElite.Services.Interfaces
{
    public interface IMarketplaceService
    {

        Task AddListingAsync(Game game, Item item);

        Task<bool> BuyItemAsync(Item item);

        Task<List<Item>> GetAllListingsAsync();

        Task<List<User>> GetAllUsersAsync();

        User GetCurrentUser();

        Task<List<Item>> GetListingsByGameAsync(Game game);

        Task RemoveListingAsync(Game game, Item item);

        void SetCurrentUser(User user);

        Task UpdateListingAsync(Game game, Item item);
    }
}