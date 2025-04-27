namespace SteamHub.Api.Context;

using Entities;
using Microsoft.EntityFrameworkCore;
using Models;

public class UserRepository : IUserRepository
{
    private readonly DataContext context;

    public UserRepository(DataContext context)
    {
        this.context = context;
    }

    public async Task<GetUsersResponse?> GetUsersAsync()
    {
        var users = await context.Users
            .Include(user => user.UserRole)
            .Select(user => new UserResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.RoleId,
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
        var result = await context.Users
            .Where(user => user.UserId == id)
            .Select(user => new UserResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.RoleId,
                WalletBalance = user.WalletBalance,
                PointsBalance = user.PointsBalance
            })
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task UpdateUserAsync(int userId, UpdateUserRequest request)
    {
        var existingUser = await context.Users.FindAsync(userId);
        if (existingUser == null)
        {
            throw new Exception("User not found");
        }

        existingUser.UserName = request.UserName;
        existingUser.Email = request.Email;
        existingUser.RoleId = request.Role;
        existingUser.WalletBalance = request.WalletBalance;
        existingUser.PointsBalance = request.PointsBalance;

        await context.SaveChangesAsync();
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
    {
        User newUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            RoleId = request.Role,
            WalletBalance = request.WalletBalance,
            PointsBalance = request.PointsBalance
        };

        await context.Users.AddAsync(newUser);

        await context.SaveChangesAsync();

        return new CreateUserResponse
        {
            UserId = newUser.UserId
        };
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}