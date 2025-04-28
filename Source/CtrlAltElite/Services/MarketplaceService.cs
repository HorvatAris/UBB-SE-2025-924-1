using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.Services
{
    using CtrlAltElite.Repositories.Interfaces;
    using CtrlAltElite.Services.Interfaces;
    using SteamStore.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IMarketplaceRepository marketplaceRepository;
        private User currentUser;

        public MarketplaceService(IMarketplaceRepository marketplaceRepository)
        {
            if (marketplaceRepository != null)
            {
                this.marketplaceRepository = marketplaceRepository;
            }
            else
            {
                throw new ArgumentNullException(nameof(marketplaceRepository));
            }

            this.currentUser = this.marketplaceRepository.GetCurrentUser();
        }

        public User GetCurrentUser()
        {
            return this.marketplaceRepository.GetCurrentUser();
        }

        public void SetCurrentUser(User user)
        {
            if (user != null)
            {
                this.currentUser = user;
            }
            else
            {
                throw new ArgumentNullException(nameof(user));
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await this.marketplaceRepository.GetAllUsersAsync();
        }

        public async Task<List<Item>> GetAllListingsAsync()
        {
            return await this.marketplaceRepository.GetAllListedItemsAsync();
        }

        public async Task<List<Item>> GetListingsByGameAsync(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            return await this.marketplaceRepository.GetListedItemsByGameAsync(game);
        }
        public async Task AddListingAsync(Game game, Item item)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.marketplaceRepository.MakeItemListableAsync(game, item);
        }

        public async Task RemoveListingAsync(Game game, Item item)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.marketplaceRepository.MakeItemListableAsync(game, item);
        }

        public async Task UpdateListingAsync(Game game, Item item)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await this.marketplaceRepository.UpdateItemPriceAsync(game, item);
        }
        public async Task<bool> BuyItemAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsListed)
            {
                throw new InvalidOperationException("Item is not listed for sale");
            }
           // item.SetIsListed(false);
            return await this.marketplaceRepository.BuyItemAsync(item, this.currentUser);
        }
    }
}
