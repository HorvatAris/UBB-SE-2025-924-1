using SteamHub.Api.Models.Item;

namespace SteamHub.Api.Service
{
    public interface IItemService
    {
        Task<ItemResponseDto> CreateItemAsync(CreateItemRequest createDto);
        Task<ItemResponseDto?> GetItemAsync(int itemId);
        Task<IEnumerable<ItemResponseDto>> GetAllItemsAsync();
        Task<ItemResponseDto> UpdateItemAsync(UpdateItemRequestDto updateDto);
        Task<bool> DeleteItemAsync(int itemId);
    }
}
