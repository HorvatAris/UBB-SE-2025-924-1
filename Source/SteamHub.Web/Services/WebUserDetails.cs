using System.Globalization;
using System.Security.Claims;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.Services;

public class WebUserDetails: IUserDetails
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public WebUserDetails(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }


    public int UserId => int.Parse(GetClaimValue(ClaimTypes.NameIdentifier)!);

    public float PointsBalance
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.Session.GetString("PointsBalance");
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : 0;
        }
        set
        {
            httpContextAccessor.HttpContext?.Session.SetString("PointsBalance",
                value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public UserRole UserRole => Enum.Parse<UserRole>(GetClaimValue(ClaimTypes.Role)!);
    public string UserName => GetClaimValue(ClaimTypes.Name)!;
    public string Email => GetClaimValue(ClaimTypes.Email)!;
    public float WalletBalance
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.Session.GetString("WalletBalance");
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : 0;
        }
        set
        {
            httpContextAccessor.HttpContext?.Session.SetString("WalletBalance",
                value.ToString(CultureInfo.InvariantCulture));
        }
    }

    private string? GetClaimValue(string claimType)
    {
        return httpContextAccessor.HttpContext!.User.FindFirst(claimType)!.Value;
    }
}