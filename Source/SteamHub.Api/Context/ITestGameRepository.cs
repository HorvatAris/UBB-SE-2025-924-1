using SteamHub.Api.Models;

namespace SteamHub.Api.Context;

public interface ITestGameRepository
{
	Task<TestGameResponse?> GetTestGameByIdAsync(int id);
	Task<GetTestGamesResponse> GetTestGamesAsync();
}