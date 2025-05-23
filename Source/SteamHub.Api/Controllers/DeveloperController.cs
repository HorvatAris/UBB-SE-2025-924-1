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

        [HttpPatch("Validate/{game_id}")]
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

        [HttpGet("RejectionMessage/{id}")]
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
        [HttpGet("Unvalidated/{userId}")]
        public async Task<IActionResult> GetUnvalidatedGamesAsync([FromRoute] int userId)
        {
            try
            {
                var games = await this.developerService.GetUnvalidatedAsync(userId);
                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("Reject/{game_id}")]
        public async Task<IActionResult> RejectGameAsync([FromRoute] int game_id)
        {
            try
            {
                await this.developerService.RejectGameAsync(game_id);
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

        [HttpPatch("RejectWithMessage/{id}")]
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

        [HttpPost("Create/{userId}")]
        public async Task<IActionResult> CreateGame([FromRoute] int userId, [FromBody] Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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

        [HttpPatch("Update/{userId}")]
        public async Task<IActionResult> UpdateGame([FromBody] Game game, [FromRoute]  int userId)
        {
            try
            {
                await developerService.UpdateGameAsync(game,userId);
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

        [HttpPatch("UpdateWithTags/{userId}")]
        public async Task<IActionResult> UpdateGameWithTags([FromRoute] int userId,[FromBody] UpdateGameWithTagsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body is null");
                }

                if (request.Game == null)
                {
                    return BadRequest("Game object is null");
                }

                if (request.SelectedTags == null)
                {
                    return BadRequest("SelectedTags is null");
                }

                System.Diagnostics.Debug.WriteLine($"Received request for userId: {userId}");
                System.Diagnostics.Debug.WriteLine($"Game ID: {request.Game.GameId}");
                System.Diagnostics.Debug.WriteLine($"Selected Tags Count: {request.SelectedTags.Count}");

                await developerService.UpdateGameWithTagsAsync(request.Game, request.SelectedTags, userId);
                return Ok("Game updated successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateGameWithTags: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [HttpDelete("Delete/{gameId}")]
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

        [HttpGet("Games/{userId}")]
        public async Task<IActionResult> GetDeveloperGames([FromRoute] int userId)
        {
            try
            {
                var games = await developerService.GetDeveloperGamesAsync(userId);
                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Games/{gameId}/Tags/{tagId}")]
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

        [HttpGet("Tags")]
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

        [HttpGet("Games/{gameId}/Exists")]
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


        [HttpGet("Games/{gameId}/Tags")]
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

        [HttpPatch("Games/{gameId}/Tags")]
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

        [HttpGet("Games/{gameId}/OwnersCount")]
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

