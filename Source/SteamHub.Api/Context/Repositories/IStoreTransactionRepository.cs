using SteamHub.Api.Models.StoreTransaction;

namespace SteamHub.Api.Context.Repositories
{
    public interface IStoreTransactionRepository
    {
        Task<CreateStoreTransactionResponse> CreateStoreTransactionAsync(CreateStoreTransactionRequest request);
        Task DeleteStoreTransactionAsync(int id);
        Task<StoreTransactionResponse?> GetStoreTransactionByIdAsync(int id);
        Task<GetStoreTransactionsResponse?> GetStoreTransactionsAsync();
        Task UpdateStoreTransactionAsync(int storeTransactionId, UpdateStoreTransactionRequest request);
    }
}