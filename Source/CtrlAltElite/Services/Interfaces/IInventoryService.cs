using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamStore.Services.Interfaces
{
    using SteamStore.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IInventoryService
    {
        Task<List<Item>> GetItemsFromInventoryAsync(Game game);

        Task<List<Item>> GetAllItemsFromInventoryAsync(User user);

        Task AddItemToInventoryAsync(Game game, Item item, User user);

        Task RemoveItemFromInventoryAsync(Game game, Item item, User user);

        Task<List<Item>> GetUserInventoryAsync(int userId);

        Task<List<User>> GetAllUsersAsync();

        Task<bool> SellItemAsync(Item item);

        List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText);

        List<Game> GetAvailableGames(List<Item> items);


        Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText);
    }
}
