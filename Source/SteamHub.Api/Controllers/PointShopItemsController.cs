using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;

namespace SteamHub.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PointShopItemsController : ControllerBase
	{
		private readonly IPointShopRepository _pointShopRepository;

		public PointShopItemsController(IPointShopRepository pointShopRepository)
		{
            _pointShopRepository = pointShopRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _pointShopRepository.GetAllItemsAsync();

			return Ok(result);
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetByUserId([FromRoute] int userId)
		{
			var result = await _pointShopRepository.GetUserItemsAsync(userId);

            if (result is null)
			{
				return NotFound();
			}

			return Ok(result);
		}
    }
}
