using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.Api.Models.StoreTransaction;

namespace SteamHub.Api.Context.Repositories;

public class StoreTransactionRepository : IStoreTransactionRepository
{
    private readonly DataContext _context;

    public StoreTransactionRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<GetStoreTransactionsResponse?> GetStoreTransactionsAsync()
    {
        var storeTransactions = await _context.StoreTransactions
            .Select(storeTransaction => new StoreTransactionResponse
            {
                StoreTransactionId = storeTransaction.StoreTransactionId,
                UserId = storeTransaction.UserId,
                GameId = storeTransaction.GameId,
                Date = storeTransaction.Date,
                Amount = storeTransaction.Amount,
                WithMoney = storeTransaction.WithMoney
            })
            .ToListAsync();

        return new GetStoreTransactionsResponse
        {
            StoreTransactions = storeTransactions
        };
    }

    public async Task<StoreTransactionResponse?> GetStoreTransactionByIdAsync(int id)
    {
        var result = await _context.StoreTransactions
            .Where(storeTransaction => storeTransaction.StoreTransactionId == id)
            .Select(storeTransaction => new StoreTransactionResponse
            {
                StoreTransactionId = storeTransaction.StoreTransactionId,
                UserId = storeTransaction.UserId,
                GameId = storeTransaction.GameId,
                Date = storeTransaction.Date,
                Amount = storeTransaction.Amount,
                WithMoney = storeTransaction.WithMoney
            })
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task UpdateStoreTransactionAsync(int storeTransactionId, UpdateStoreTransactionRequest request)
    {
        var existingStoreTransaction = await _context.StoreTransactions.FindAsync(storeTransactionId);
        if (existingStoreTransaction == null)
        {
            throw new Exception("StoreTransaction not found");
        }

        existingStoreTransaction.StoreTransactionId = storeTransactionId;
        existingStoreTransaction.Date = request.Date;
        existingStoreTransaction.Amount = request.Amount;
        existingStoreTransaction.WithMoney = request.WithMoney;

        await _context.SaveChangesAsync();
    }

    public async Task<CreateStoreTransactionResponse> CreateStoreTransactionAsync(CreateStoreTransactionRequest request)
    {
        StoreTransaction newStoreTransaction = new StoreTransaction
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Date = request.Date,
            Amount = request.Amount,
            WithMoney = request.WithMoney
        };

        await _context.StoreTransactions.AddAsync(newStoreTransaction);

        await _context.SaveChangesAsync();

        return new CreateStoreTransactionResponse
        {
            StoreTransactionId = newStoreTransaction.StoreTransactionId
        };
    }

    public async Task DeleteStoreTransactionAsync(int id)
    {
        var storeTransaction = await _context.StoreTransactions.FindAsync(id);
        if (storeTransaction == null)
        {
            throw new Exception("StoreTransaction not found");
        }
        _context.StoreTransactions.Remove(storeTransaction);
        await _context.SaveChangesAsync();
    }

}
