﻿using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Controllers;

using Context;
using Microsoft.AspNetCore.Mvc;
using Models;
using SteamHub.Api.Context.Repositories;
using SteamHub.ApiContract.Models.Game;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IGameRepository gameRepository;

    public GamesController(IGameRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    // GET: api/games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDetailedResponse>>> GetGames([FromQuery] GetGamesRequest request)
    {
        var games = await this.gameRepository.GetGamesAsync(request);

        return Ok(games);
    }

    // GET: api/games/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GameDetailedResponse>> GetGameById([FromRoute] int id)
    {
        var game = await this.gameRepository.GetGameByIdAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    [HttpPatch("{id}/tags")]
    public async Task<IActionResult> PatchTags([FromRoute] int id, [FromBody] PatchGameTagsRequest tags)
    {
        var game = await gameRepository.GetGameByIdAsync(id);

        if (game == null)
        {
            return NotFound();
        }
        await gameRepository.PatchGameTagsAsync(id, tags);

        return NoContent();
    }

    // POST: api/games
    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest game)
    {
        var createdGame = await this.gameRepository.CreateGameAsync(game);

        // Returns a 201 Created response along with the created game
        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Identifier }, createdGame);
    }

    // PATCH: api/games/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateGame([FromRoute] int id, [FromBody] UpdateGameRequest game)
    {
        await this.gameRepository.UpdateGameAsync(id, game);

        return NoContent();
    }

    // DELETE: api/games/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame([FromRoute] int id)
    {
        var game = await this.gameRepository.GetGameByIdAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        // Delete the game from the repository
        await this.gameRepository.DeleteGameAsync(id);

        return NoContent(); // Return a 204 NoContent response
    }
}