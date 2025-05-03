using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Entities;
using SteamHub.Api.Models.StoreTransaction;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreTransactionsController : ControllerBase
    {
        private readonly IStoreTransactionRepository _storeTransactionRepository;

        public StoreTransactionsController(IStoreTransactionRepository storeTransactionRepository)
        {
            _storeTransactionRepository = storeTransactionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _storeTransactionRepository.GetStoreTransactionsAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _storeTransactionRepository.GetStoreTransactionByIdAsync(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStoreTransactionRequest request)
        {
            try
            {
                await _storeTransactionRepository.UpdateStoreTransactionAsync(id, request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStoreTransactionAsync([FromBody] CreateStoreTransactionRequest request)
        {
            try
            {
                var createdStoreTransaction = await _storeTransactionRepository.CreateStoreTransactionAsync(request);
                return Ok(createdStoreTransaction);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoreTransactionAsync([FromRoute] int id)
        {
            try
            {
                await _storeTransactionRepository.DeleteStoreTransactionAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }
    }
}