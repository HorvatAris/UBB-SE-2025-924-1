using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;
using SteamHub.Api.Models;

namespace SteamHub.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _gameRepository;

    // Inject the repository through the constructor
    public GamesController(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    // GET: api/games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames([FromQuery]GamesQueryParams queryParams)
    {
        var games = await _gameRepository.GetGamesAsync(queryParams);

        return Ok(games);
    }

    // GET: api/games/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGameById(int id)
    {
        var game = await _gameRepository.GetGameByIdAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    // GET: api/games/{id}
    [HttpPatch("{id}/tags")]
    public async Task<ActionResult> PatchTags(int id, [FromBody]GameTagsPatch tags)
    {
        var game = await _gameRepository.GetGameByIdAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        if (tags.Type == GameTagsPatchType.Insert)
        {
            await _gameRepository.InsertGameTag(id, tags.TagIds.ToArray());
        }
        else
        {
            await _gameRepository.DeleteGameTag(id, tags.TagIds.ToArray());
        }

        return NoContent(); // Return a 204 NoContent response
    }

    // POST: api/games
    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] Game game)
    {
        game.Tags = new HashSet<Tag>();

        var createdGame = await _gameRepository.CreateGameAsync(game);

        // Returns a 201 Created response along with the created game
        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Identifier }, createdGame);
    }

    // PUT: api/games/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] Game game)
    {

        game.Identifier = id;
        game.Tags = new HashSet<Tag>();

        await _gameRepository.UpdateGameAsync(game);

        return NoContent();
    }

    // DELETE: api/games/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await _gameRepository.GetGameByIdAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        // Delete the game from the repository
        await _gameRepository.DeleteGameAsync(id);

        return NoContent(); // Return a 204 NoContent response
    }
}