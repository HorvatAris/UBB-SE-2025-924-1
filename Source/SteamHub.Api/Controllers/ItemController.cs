using System.Threading.Tasks;
using SteamHub.Api.Models.Item;
using SteamHub.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // POST api/items
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _itemService.CreateItemAsync(createDto);
            return CreatedAtAction(nameof(GetItem), new { id = createdItem.ItemId }, createdItem);
        }

        // GET api/items/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _itemService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // GET api/items
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllItemsAsync();
            return Ok(items);
        }

        // PUT api/items/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemRequestDto updateDto)
        {
            if (id != updateDto.ItemId)
            {
                return BadRequest("Item id mismatch");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedItem = await _itemService.UpdateItemAsync(updateDto);
                return Ok(updatedItem);
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE api/items/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var success = await _itemService.DeleteItemAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
    

}
