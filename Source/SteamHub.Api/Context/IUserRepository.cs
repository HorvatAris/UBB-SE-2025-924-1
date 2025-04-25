namespace SteamHub.Api.Context
{
    using Models;

    public interface IUserRepository
    {
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
        Task DeleteUserAsync(int id);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<GetUsersResponse?> GetUsersAsync();
        Task UpdateUserAsync(int userId, UpdateUserRequest request);
    }
}