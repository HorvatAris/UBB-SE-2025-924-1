using CtrlAltElite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.Services.Interfaces
{
    public interface ITradeService
    {
        Task<List<ItemTrade>> GetActiveTradesAsync(int userId);

        User GetCurrentUser();

        Task AddItemTradeAsync(ItemTrade trade);
    }
}
