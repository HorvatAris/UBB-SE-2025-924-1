namespace SteamHub.Api.Service
{
    using SteamHub.Api.Context.Repositories;
    using SteamHub.Api.Entities;
    using SteamHub.Api.Models.Game;
    using SteamHub.Api.Models.Item;

    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IGameRepository _gameRepository;

        public ItemService(IItemRepository itemRepository, IGameRepository gameRepository)
        {
            _itemRepository = itemRepository;
            _gameRepository = gameRepository;
        }

        public async Task<ItemResponseDto> CreateItemAsync(CreateItemRequest createDto)
        {
            var game = await _gameRepository.GetGameEntityByIdAsync(createDto.GameId);
            if (game == null)
            {
                throw new ArgumentException($"Game with id {createDto.GameId} not found.");
            }

            var item = new Item(createDto.ItemName, game, createDto.Price, createDto.Description);

            item = await _itemRepository.AddItemAsync(item);

            item.ImagePath = GenerateDefaultImagePath(item);
            await _itemRepository.UpdateItemAsync(item);

            return MapToResponseDto(item);
        }

        public async Task<ItemResponseDto?> GetItemAsync(int itemId)
        {
            var item = await _itemRepository.GetItemAsync(itemId);
            if (item == null)
            {
                return null;
            }
            return MapToResponseDto(item);
        }

        public async Task<IEnumerable<ItemResponseDto>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllItemsAsync();
            return items.Select(i => MapToResponseDto(i)).ToList();
        }

        public async Task<ItemResponseDto> UpdateItemAsync(UpdateItemRequestDto updateDto)
        {
            var item = await _itemRepository.GetItemAsync(updateDto.ItemId);
            if (item == null)
            {
                throw new ArgumentException($"Item with id {updateDto.ItemId} not found.");
            }

            item.ItemName = updateDto.ItemName;
            item.Price = updateDto.Price;
            item.Description = updateDto.Description;
            item.IsListed = updateDto.IsListed;
            item.ImagePath = updateDto.ImagePath;

            await _itemRepository.UpdateItemAsync(item);
            return MapToResponseDto(item);
        }

        public async Task<bool> DeleteItemAsync(int itemId)
        {
            var item = await _itemRepository.GetItemAsync(itemId);
            if (item == null)
            {
                return false;
            }

            await _itemRepository.DeleteItemAsync(item);
            return true;
        }

        private string GenerateDefaultImagePath(Item item)
        {
            string gameFolder = GameFolderResolver.GetFolderName(item.Game.Name);
            return $"ms-appx:///Assets/img/games/{gameFolder}/{item.ItemId}.png";
        }

        private ItemResponseDto MapToResponseDto(Item item)
        {
            return new ItemResponseDto
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                GameTitle = item.Game.Name,
                Price = item.Price,
                Description = item.Description,
                IsListed = item.IsListed,
                ImagePath = item.ImagePath
            };
        }
    }
}
