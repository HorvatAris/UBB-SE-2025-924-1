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

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UserResponse updatedUserResponse)
        {
            User updatedUser = new User
            {
                UserId = id,
                UserName = updatedUserResponse.UserName,
                Email = updatedUserResponse.Email,
                UserRole = updatedUserResponse.UserRole,
                WalletBalance = updatedUserResponse.WalletBalance,
                PointsBalance = updatedUserResponse.PointsBalance
            };

            var success = await _userRepository.UpdateUserAsync(updatedUser);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserResponse newUser)
        {
            if (newUser == null)
            {
                return BadRequest("User data is required.");
            }
            User user = new User
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                UserRole = newUser.UserRole,
                WalletBalance = newUser.WalletBalance,
                PointsBalance = newUser.PointsBalance
            };
            await _userRepository.CreateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userRepository.DeleteUserAsync(id);
            return NoContent();

        }
    }
}