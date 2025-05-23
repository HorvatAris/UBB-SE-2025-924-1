using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService gameService;

        public GameController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        // GET: api/game
        [HttpGet]
        public async Task<IActionResult> GetGames()
        {
            var games = await this.gameService.GetAllGamesAsync();

            return Ok(games);
        }

        // GET: api/game/tags
        [HttpGet("Tags")]
        public async Task<IActionResult> GetTags()
        {
            var games = await this.gameService.GetAllTagsAsync();

            return Ok(games);
        }

        // GET: api/game/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameById([FromRoute] int id)
        {
            var game = await this.gameService.GetGameByIdAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }
    }
}
