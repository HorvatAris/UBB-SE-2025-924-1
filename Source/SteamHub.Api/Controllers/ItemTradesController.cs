namespace SteamHub.Api.Controllers
{
    using Context;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [ApiController]
    [Route("api/[controller]")]
    public class ItemTradesController : ControllerBase
    {
        private readonly IItemTradeRepository itemTradeRepository;

        public ItemTradesController(IItemTradeRepository itemTradeRepository)
        {
            this.itemTradeRepository = itemTradeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await itemTradeRepository.GetItemTradesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await itemTradeRepository.GetItemTradeByIdAsync(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateItemTradeRequest request)
        {
            try
            {
                await itemTradeRepository.UpdateItemTradeAsync(id, request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemTradeAsync([FromBody] CreateItemTradeRequest request)
        {
            try
            {
                var newTrade = await itemTradeRepository.CreateItemTradeAsync(request);
                return Ok(newTrade);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemTradeAsync([FromRoute] int id)
        {
            try
            {
                await itemTradeRepository.DeleteItemTradeAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }
    }
}
