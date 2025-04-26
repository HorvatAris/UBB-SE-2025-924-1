using SteamHub.Api.Entities;

namespace SteamHub.Api.Models;

public class GetUsersResponse
{
	public List<UserResponse> Users { get; set; }
}
