using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context;

namespace SteamHub.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TestGamesController : ControllerBase
	{
		private readonly ITestGameRepository _testGameRepository;

		public TestGamesController(ITestGameRepository testGameRepository)
		{
			_testGameRepository = testGameRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _testGameRepository.GetTestGamesAsync();

			return Ok(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById([FromRoute] int id)
		{
			var result = await _testGameRepository.GetTestGameByIdAsync(id);

			if (result is null)
			{
				return NotFound();
			}

			return Ok(result);
		}
	}
}
