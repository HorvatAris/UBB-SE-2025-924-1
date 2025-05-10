namespace SteamHub.Web.Services;

public interface IAuthManager
{
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
}