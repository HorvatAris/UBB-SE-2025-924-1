using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.Developer;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using System.Collections.ObjectModel;

namespace SteamHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperController : ControllerBase
    {
        private readonly IDeveloperService developerService;
        public DeveloperController(IDeveloperService developerService)
        {
            this.developerService = developerService;
        }

        //Task ValidateGameAsync(int game_id);

        [HttpPatch("games/validate/{game_id}")]
        public async Task<IActionResult> ValidateGameAsync([FromRoute] int game_id)
        {
            try
            {
                await this.developerService.ValidateGameAsync(game_id);
                return Ok(new { message = "Game validated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("games/{id}/rejection-message")]
        public async Task<IActionResult> GetRejectionMessageAsync([FromRoute] int id)
        {
            try
            {
                var message = await this.developerService.GetRejectionMessageAsync(id);
                return Ok(message); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("games/unvalidated")]
        public async Task<IActionResult> GetUnvalidatedGamesAsync()
        {
            try
            {
                var games = await this.developerService.GetUnvalidatedAsync();
                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("games/{id}/reject")]
        public async Task<IActionResult> RejectGameAsync([FromRoute] int id)
        {
            try
            {
                await this.developerService.RejectGameAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("games/{id}/reject-with-message")]
        public async Task<IActionResult> RejectGameWithMessageAsync([FromRoute]int id, [FromBody] string message)
        {
            try
            {
                await this.developerService.RejectGameWithMessageAsync(id, message);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("games/{userId}/create")]
        public async Task<IActionResult> CreateGame([FromRoute] int userId, [FromBody] Game game)
        {
            try
            {
                await this.developerService.CreateGameAsync(game, userId);
                return Ok(new { message = "Game created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }
        [HttpPatch("games/update")]
        public async Task<IActionResult> UpdateGame([FromBody] Game game)
        {
            try
            {
                await developerService.UpdateGameAsync(game);
                return Ok(new { message = "Game updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("games/update-with-tags")]
        public async Task<IActionResult> UpdateGameWithTags([FromBody] UpdateGameWithTagsRequest request)
        {
            try
            {
                await developerService.UpdateGameWithTagsAsync(request.Game, request.SelectedTags);
                return Ok(new { message = "Game and tags updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpDelete("games/{gameId}")]
        public async Task<IActionResult> DeleteGame([FromRoute] int gameId)
        {
            try
            {
                await developerService.DeleteGameAsync(gameId);
                return Ok(new { message = $"Game with ID {gameId} deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("games/developer")]
        public async Task<IActionResult> GetDeveloperGames()
        {
            try
            {
                var games = await developerService.GetDeveloperGamesAsync();
                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("games/{gameId}/tags/{tagId}")]
        public async Task<IActionResult> InsertGameTag([FromRoute] int gameId, [FromRoute] int tagId)
        {
            try
            {
                await developerService.InsertGameTagAsync(gameId, tagId);
                return Ok(new { message = "Tag inserted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("tags")]
        public async Task<IActionResult> GetAllTags()
        {
            try
            {
                var tags = await developerService.GetAllTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("games/{gameId}/exists")]
        public async Task<IActionResult> IsGameIdInUse([FromRoute] int gameId)
        {
            try
            {
                bool exists = await developerService.IsGameIdInUseAsync(gameId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("games/{gameId}/tags")]
        public async Task<IActionResult> GetGameTags([FromRoute] int gameId)
        {
            try
            {
                var tags = await developerService.GetGameTagsAsync(gameId);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPatch("games/{gameId}/tags")]
        public async Task<IActionResult> DeleteGameTags([FromRoute] int gameId)
        {
            try
            {
                await developerService.DeleteGameTagsAsync(gameId);
                return Ok(new { message = "Game tags deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("games/{gameId}/owners/count")]
        public async Task<IActionResult> GetGameOwnerCount([FromRoute] int gameId)
        {
            try
            {
                var count = await developerService.GetGameOwnerCountAsync(gameId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}

