using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using SteamHub.ApiContract.Models.Login;

namespace SteamHub.Web.Services
{

    public class AuthManager : IAuthManager
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthManager(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            httpClient = httpClientFactory.CreateClient("AuthApi");
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginModel = new { Username = username, Password = password };

            var response = await httpClient.PostAsJsonAsync("api/Authentication/Login", loginModel);
            
            if (!response.IsSuccessStatusCode)
                return false;
            
            var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
            
            if (content == null)
                return false;
            
            // Create claims from API response
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, content.UserId.ToString()),
                new Claim(ClaimTypes.Name, content.Username),
                new Claim(ClaimTypes.Email, content.Email),
                new Claim(ClaimTypes.Role, content.UserRole)
            };

            

            // Create the identity
            var identity = new ClaimsIdentity(claims, "SteamHubAuth");
            var principal = new ClaimsPrincipal(identity);

            // Sign in
            await httpContextAccessor.HttpContext.SignInAsync(
                "SteamHubAuth",
                principal);
            httpContextAccessor.HttpContext.Session.SetString("PointsBalance", content.PointsBalance.ToString(CultureInfo.InvariantCulture));
            httpContextAccessor.HttpContext.Session.SetString("WalletBalance", content.WalletBalance.ToString(CultureInfo.InvariantCulture));
            return true;
        }

        public async Task LogoutAsync()
        {
            await httpContextAccessor.HttpContext.SignOutAsync("SteamHubAuth");
        }
    }


}