using Microsoft.AspNetCore.Mvc;
using SteamHub.Api.Context;
using SteamHub.Api.Models;

namespace SteamHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
	private readonly ITagRepository _tagRepository;

	public TagsController(ITagRepository tagRepository)
	{
		_tagRepository = tagRepository;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var result = await _tagRepository.GetAllTagsAsync();

		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById([FromRoute] int id)
	{
		var result = await _tagRepository.GetTagByIdAsync(id);

		if (result is null)
		{
			return NotFound();
		}

		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
	{
		try
		{
			await _tagRepository.CreateTagAsync(request);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}

		return NoContent();
	}


	[HttpPatch("{id}")]
	public async Task<IActionResult> CreateTag([FromRoute] int id, [FromBody] UpdateTagRequest request)
	{
		try
		{
			await _tagRepository.UpdateTagAsync(id, request);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}

		return NoContent();
	}


	[HttpDelete("{id}")]
	public async Task<IActionResult> CreateTag([FromRoute] int id)
	{
		try
		{
			await _tagRepository.DeleteTagAsync(id);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}

		return NoContent();
	}
}
