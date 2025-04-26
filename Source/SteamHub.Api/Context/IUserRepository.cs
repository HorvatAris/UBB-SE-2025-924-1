using SteamHub.Api.Entities;
using SteamHub.Api.Models;

namespace SteamHub.Api.Context
{
    public interface IUserRepository
    {
        Task<bool> CreateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<GetUsersResponse?> GetUsersAsync();
        Task<bool> UpdateUserAsync(User user);
    }
}