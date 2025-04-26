using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.Api.Models;

namespace SteamHub.Api.Context;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        this._context = context;
    }
    public async Task<GetUsersResponse?> GetUsersAsync()
    {
        var users = await _context.Users
            .Select(user => new UserResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                UserRole = user.UserRole,
                WalletBalance = user.WalletBalance,
                PointsBalance = user.PointsBalance
            })
            .ToListAsync();

        return new GetUsersResponse
        {
            Users = users
        };
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var result = await _context.Users
            .Where(user => user.UserId == id)
            .Select(user => new UserResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                UserRole = user.UserRole,
                WalletBalance = user.WalletBalance,
                PointsBalance = user.PointsBalance
            })
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var existingUser = await _context.Users.FindAsync(user.UserId);
        if (existingUser == null)
            return false;

        existingUser.UserName = user.UserName;
        existingUser.Email = user.Email;
        existingUser.UserRole = user.UserRole;
        existingUser.WalletBalance = user.WalletBalance;
        existingUser.PointsBalance = user.PointsBalance;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

}
