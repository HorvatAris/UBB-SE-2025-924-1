namespace SteamHub.Api.Controllers
{
    using System.Threading.Tasks;
    using SteamHub.Api.Models.Item;
    using Microsoft.AspNetCore.Mvc;
    using SteamHub.Api.Context.Repositories;
    using SteamHub.Api.Entities;

    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;

        public ItemsController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        // POST: api/items
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map request DTO to Item entity.
            var newItem = new Item
            {
                ItemName = dto.ItemName,
                // Map the provided game id to the CorrespondingGameId property.
                CorrespondingGameId = dto.GameId,
                Price = dto.Price,
                Description = dto.Description,
                // ImagePath is now a URL that the client provides.
                ImagePath = dto.ImagePath,
                IsListed = false // Default value; update as needed.
            };

            var createdItem = await _itemRepository.AddItemAsync(newItem);

            // Map the entity to a response DTO.
            var response = new ItemResponse
            {
                ItemId = createdItem.ItemId,
                ItemName = createdItem.ItemName,
                GameId = createdItem.CorrespondingGameId,
                Price = createdItem.Price,
                Description = createdItem.Description,
                IsListed = createdItem.IsListed,
                ImagePath = createdItem.ImagePath
            };

            return CreatedAtAction(nameof(GetItem), new { id = response.ItemId }, response);
        }

        // GET: api/items/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _itemRepository.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var response = new ItemResponse
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                GameId = item.CorrespondingGameId,
                Price = item.Price,
                Description = item.Description,
                IsListed = item.IsListed,
                ImagePath = item.ImagePath
            };

            return Ok(response);
        }

        // GET: api/items
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemRepository.GetAllItemsAsync();
            var response = items.Select(item => new ItemResponse
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                GameId = item.CorrespondingGameId,
                Price = item.Price,
                Description = item.Description,
                IsListed = item.IsListed,
                ImagePath = item.ImagePath
            }).ToList();

            return Ok(response);
        }

        // PUT: api/items/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemRequestDto dto)
        {
            if (id != dto.ItemId)
            {
                return BadRequest("Item id mismatch.");
            }

            var existingItem = await _itemRepository.GetItemAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Update entity with the new values from the request.
            existingItem.ItemName = dto.ItemName;
            existingItem.CorrespondingGameId = dto.GameId;
            existingItem.Price = dto.Price;
            existingItem.Description = dto.Description;
            existingItem.IsListed = dto.IsListed;
            existingItem.ImagePath = dto.ImagePath;

            var updatedItem = await _itemRepository.UpdateItemAsync(existingItem);

            var response = new ItemResponse
            {
                ItemId = updatedItem.ItemId,
                ItemName = updatedItem.ItemName,
                GameId = updatedItem.CorrespondingGameId,
                Price = updatedItem.Price,
                Description = updatedItem.Description,
                IsListed = updatedItem.IsListed,
                ImagePath = updatedItem.ImagePath
            };

            return Ok(response);
        }

        // DELETE: api/items/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var existingItem = await _itemRepository.GetItemAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            await _itemRepository.DeleteItemAsync(existingItem);
            return NoContent();
        }
    }
}
