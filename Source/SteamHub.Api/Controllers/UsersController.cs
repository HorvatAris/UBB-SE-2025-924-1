using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;

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
			var result = await _userRepository.GetAllUsersAsync();

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
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserId)
            {
                return BadRequest("User ID mismatch.");
            }

            var success = await _userRepository.UpdateUserAsync(updatedUser);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
