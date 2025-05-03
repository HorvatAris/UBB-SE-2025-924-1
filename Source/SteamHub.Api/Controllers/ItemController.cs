namespace SteamHub.Api.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SteamHub.Api.Context.Repositories;
    using SteamHub.Api.Entities;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Models.Item;

    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository itemRepository;

        public ItemsController(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        // GET: api/items?...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDetailedResponse>>> GetItems()
        {
            var items = await this.itemRepository.GetItemsAsync();
            return Ok(items);
        }

        // GET: api/items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDetailedResponse>> GetItemById([FromRoute] int id)
        {
            var item = await this.itemRepository.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST: api/items
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request)
        {
            var createdItem = await this.itemRepository.CreateItemAsync(request);
            return CreatedAtAction(nameof(GetItemById), new { id = createdItem.ItemId }, createdItem);
        }

        // PUT: api/items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem([FromRoute] int id, [FromBody] UpdateItemRequest request)
        {
            await this.itemRepository.UpdateItemAsync(id, request);
            return NoContent();
        }

        // DELETE: api/items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem([FromRoute] int id)
        {
            var item = await this.itemRepository.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await this.itemRepository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}
