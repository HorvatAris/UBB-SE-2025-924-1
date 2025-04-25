using Microsoft.Data.SqlClient;
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

    /// <summary>
    /// Asynchronously retrieves all users from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="User"/> objects.</returns>
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await this._context.Users.ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the <see cref="User"/> object,
    /// or <c>null</c> if no user with the specified ID is found.
    /// </returns>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    /// <summary>
    /// Asynchronously updates the specified user's data in the database.
    /// </summary>
    /// <param name="user">The user object containing updated information.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result indicates whether the update was successful.
    /// </returns>
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

}
