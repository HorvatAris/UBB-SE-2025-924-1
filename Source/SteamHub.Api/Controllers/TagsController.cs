namespace SteamHub.Api.Controllers;

using Context;
using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
	private readonly ITagRepository tagRepository;

	public TagsController(ITagRepository tagRepository)
	{
		this.tagRepository = tagRepository;
	}

	[HttpGet]
	public async Task<ActionResult<GetTagsResponse>> GetAll()
	{
		var result = await tagRepository.GetAllTagsAsync();

		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<TagNameOnlyResponse>> GetById([FromRoute] int id)
	{
		var result = await tagRepository.GetTagByIdAsync(id);

		if (result is null)
		{
			return NotFound();
		}

		return Ok(result);
	}

	[HttpPost]
	public async Task<ActionResult<CreateTagResponse>> CreateTag([FromBody] CreateTagRequest request)
	{
		try
		{
			var createdTag = await tagRepository.CreateTagAsync(request);

			return Ok(createdTag);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpPatch("{id}")]
	public async Task<IActionResult> UpdateTag([FromRoute] int id, [FromBody] UpdateTagRequest request)
	{
		try
		{
			await tagRepository.UpdateTagAsync(id, request);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteTag([FromRoute] int id)
	{
		try
		{
			await tagRepository.DeleteTagAsync(id);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}

		return NoContent();
	}
}
