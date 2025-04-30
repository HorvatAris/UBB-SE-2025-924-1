using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Models.ItemTradeDetails;

namespace SteamHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemTradeDetailsController : ControllerBase
{
    private readonly IItemTradeDetailRepository _repository;

    public ItemTradeDetailsController(IItemTradeDetailRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _repository.GetItemTradeDetailsAsync();
        return Ok(result);
    }

    [HttpGet("{tradeId}/{itemId}")]
    public async Task<IActionResult> GetById(int tradeId, int itemId)
    {
        var result = await _repository.GetItemTradeDetailAsync(tradeId, itemId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItemTradeDetailRequest request)
    {
        try
        {
            var created = await _repository.CreateItemTradeDetailAsync(request);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }

    [HttpDelete("{tradeId}/{itemId}")]
    public async Task<IActionResult> Delete(int tradeId, int itemId)
    {
        try
        {
            await _repository.DeleteItemTradeDetailAsync(tradeId, itemId);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }

        return NoContent();
    }
}
