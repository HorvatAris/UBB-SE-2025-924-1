using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context;
using SteamHub.ApiContract.Models.Login;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly DataContext dataContext;

    public AuthenticationController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [HttpPost("Login")]
    public ActionResult<LoginResponse> Login([FromBody]LoginRequest loginRequest)
    {
        var user = dataContext.Users.FirstOrDefault(u => u.UserName == loginRequest.Username);
        if (user == null)
        {
            return Unauthorized();
        }

        if (loginRequest.Password != "secret")
        {
            return Unauthorized();
        }

        var response = new LoginResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            UserRole = user.RoleId.ToString(),
            Username = user.UserName,
            WalletBalance = user.WalletBalance,
            PointsBalance = user.PointsBalance
        };
        return Ok(response);
    }
}