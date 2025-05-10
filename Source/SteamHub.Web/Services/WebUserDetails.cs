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
        get => float.Parse(httpContextAccessor.HttpContext!.Session.GetString("PointsBalance")!);
        set
        {
            httpContextAccessor.HttpContext!.Session.SetString("PointsBalance",
                value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public UserRole UserRole => Enum.Parse<UserRole>(GetClaimValue(ClaimTypes.Role)!);
    public string UserName => GetClaimValue(ClaimTypes.Name)!;
    public string Email => GetClaimValue(ClaimTypes.Email)!;
    public float WalletBalance => float.Parse(httpContextAccessor.HttpContext!.Session.GetString("WalletBalance")!);

    private string? GetClaimValue(string claimType)
    {
        return httpContextAccessor.HttpContext!.User.FindFirst(claimType)!.Value;
    }
}