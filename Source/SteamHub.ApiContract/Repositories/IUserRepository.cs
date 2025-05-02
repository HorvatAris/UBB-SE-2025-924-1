namespace SteamHub.ApiContract.Repositories
{
    using SteamHub.ApiContract.Models.User;

    public interface IUserRepository
    {
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
        Task DeleteUserAsync(int id);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<GetUsersResponse?> GetUsersAsync();
        Task UpdateUserAsync(int userId, UpdateUserRequest request);
    }
}