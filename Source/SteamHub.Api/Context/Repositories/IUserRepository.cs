using SteamHub.Api.Models.User;

namespace SteamHub.Api.Context.Repositories
{
    public interface IUserRepository
    {
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<GetUsersResponse?> GetUsersAsync();
        Task UpdateUserAsync(int userId, UpdateUserRequest request);
    }
}