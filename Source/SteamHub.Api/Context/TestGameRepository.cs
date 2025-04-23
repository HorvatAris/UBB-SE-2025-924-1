using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Models;

namespace SteamHub.Api.Context;

public class TestGameRepository : ITestGameRepository
{
	private readonly DataContext _context;

	public TestGameRepository(DataContext context)
	{
		_context = context;
	}

	public async Task<TestGameResponse?> GetTestGameByIdAsync(int id)
	{
		var result = await _context.TestGames
			.Where(testGame => testGame.Id == id)
			.Select(testGame => new TestGameResponse
			{
				Name = testGame.Name
			})
			.SingleOrDefaultAsync();

		return result;
	}

	public async Task<GetTestGamesResponse> GetTestGamesAsync()
	{
		var testGames = await _context.TestGames
			.Select(testGame => new TestGameResponse
			{
				Name = testGame.Name
			})
			.ToListAsync();

		return new GetTestGamesResponse
		{
			TestGames = testGames
		};
	}
}
