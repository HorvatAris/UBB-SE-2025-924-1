using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;
using SteamHub.Api.Models;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userRepository.GetUsersAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _userRepository.GetUserByIdAsync(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                await _userRepository.UpdateUserAsync(id, request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            try
            {
                var existingUser = await _userRepository.CreateUserAsync(request);
                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
        {
            try
            {
                await _userRepository.DeleteUserAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }
    }
}