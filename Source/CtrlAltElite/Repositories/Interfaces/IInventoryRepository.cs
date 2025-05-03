using CtrlAltElite.Models;
using SteamStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamStore.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<List<Item>> GetItemsFromInventoryAsync(Game game);

        Task<List<Item>> GetUserInventoryAsync(int userId);

        Task<List<Item>> GetAllItemsFromInventoryAsync(User user);

        Task AddItemToInventoryAsync(Game game, Item item, User user);

        Task RemoveItemFromInventoryAsync(Game game, Item item, User user);

        Task<List<User>> GetAllUsersAsync();

        Task<bool> SellItemAsync(Item item);
    }
}