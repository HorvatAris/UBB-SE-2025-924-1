using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamStore.Services.Interfaces
{
    using CtrlAltElite.Models;
    using SteamStore.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IInventoryService
    {
        Task<List<Item>> GetItemsFromInventoryAsync(Game game);

        Task<List<Item>> GetAllItemsFromInventoryAsync();

        Task AddItemToInventoryAsync(Game game, Item item);

        Task RemoveItemFromInventoryAsync(Game game, Item item);

        Task<List<Item>> GetUserInventoryAsync(int userId);

        User GetAllUsersAsync();

        Task<bool> SellItemAsync(Item item);

        List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText);

        List<Game> GetAvailableGames(List<Item> items);


        Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText);
    }
}