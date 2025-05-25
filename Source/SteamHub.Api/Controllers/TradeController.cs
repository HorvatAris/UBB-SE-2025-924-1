using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Services.Interfaces;
using System.Diagnostics;

namespace SteamHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService tradeService;

        public TradeController(ITradeService tradeService)
        {
            this.tradeService = tradeService;
        }

        [HttpGet("Active/{userId}")]
        public async Task<IActionResult> GetActive([FromRoute] int userId)
        {
            var result = await tradeService.GetActiveTradesAsync(userId);
            return Ok(result);
        }

        [HttpGet("History/{userId}")]
        public async Task<IActionResult> GetHistory([FromRoute] int userId)
        {
            var result = await tradeService.GetTradeHistoryAsync(userId);
            return Ok(result);
        }

        [HttpGet("Inventory/{userId}")]
        public async Task<IActionResult> GetInventory([FromRoute] int userId)
        {
            var result = await tradeService.GetUserInventoryAsync(userId);
            return Ok(result);
        }

        [HttpPatch("Complete/{tradeId}")]
        public async Task<IActionResult> CompleteTrade([FromRoute] int tradeId)
        {
            try
            {
                await tradeService.MarkTradeAsCompletedAsync(tradeId);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddTrade([FromBody] ItemTrade trade)
        {
            try
            {
                await tradeService.AddItemTradeAsync(trade);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return Ok("Trade Created Succesfully");
        }

        [HttpPatch("Decline")]
        public async Task<IActionResult> DeclineTrade([FromBody] ItemTrade trade)
        {
            try
            {
                await tradeService.DeclineTradeRequest(trade);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return Ok("Trade Created Succesfully");
        }

        [HttpPatch("Update")]
        public async Task<IActionResult> UpdateTrade([FromBody] ItemTrade trade)
        {
            try
            {
                await tradeService.UpdateItemTradeAsync(trade);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return Ok("Trade Created Succesfully");
        }

        [HttpPatch("TransferItem")]
        public async Task<IActionResult> TransferItem([FromBody] TransferItemTradeRequest tradeRequest)
        {
            try
            {
                await tradeService.TransferItemAsync(tradeRequest);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return Ok("Item Transfered Succesfully");
        }
    }
}
