using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Context.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<GetUsersResponse?> GetUsersAsync()
    {
        var users = await _context.Users
            .Include(user => user.UserRole)
            .Select(user => new UserResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = (ApiContract.Models.User.RoleEnum)user.RoleId,
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
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = (ApiContract.Models.User.RoleEnum)user.RoleId,
                WalletBalance = user.WalletBalance,
                PointsBalance = user.PointsBalance
            })
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task UpdateUserAsync(int userId, UpdateUserRequest request)
    {
        var existingUser = await _context.Users.FindAsync(userId);
        if (existingUser == null)
        {
            throw new Exception("User not found");
        }

        existingUser.UserName = request.UserName;
        existingUser.Email = request.Email;
        existingUser.RoleId = (Entities.RoleEnum)request.Role;
        existingUser.WalletBalance = request.WalletBalance;
        existingUser.PointsBalance = request.PointsBalance;

        await _context.SaveChangesAsync();
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
    {
        User newUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            RoleId = (Entities.RoleEnum)request.Role,
            WalletBalance = request.WalletBalance,
            PointsBalance = request.PointsBalance
        };

        await _context.Users.AddAsync(newUser);

        await _context.SaveChangesAsync();

        return new CreateUserResponse
        {
            UserId = newUser.UserId
        };
    }
}
