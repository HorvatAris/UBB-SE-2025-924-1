using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.Api.Models;

namespace SteamHub.Api.Context;

public class TagRepository : ITagRepository
{
	private readonly DataContext _context;

	public TagRepository(DataContext context)
	{
		_context = context;
	}

	public async Task CreateTagAsync(CreateTagRequest request)
	{
		var isDuplicate = await _context.Tags
			.AnyAsync(tag => tag.TagName == request.TagName);

		if (isDuplicate)
		{
			throw new ArgumentException($"Tag with name {request.TagName} already exists");
		}

		_context.Add(new Tag
		{
			TagName = request.TagName
		});

		await _context.SaveChangesAsync();
	}

	public async Task<TagResponse?> GetTagByIdAsync(int tagId)
	{
		var foundTag = await _context.Tags
			.Where(tag => tag.TagId == tagId)
			.Select(tag => new TagResponse
			{
				TagName = tag.TagName
			})
			.SingleOrDefaultAsync();

		return foundTag;
	}

	public async Task<List<TagDetailedResponse>> GetAllTagsAsync()
	{
		var tags = await _context.Tags
			.Select(tag => new TagDetailedResponse
			{
				TagId = tag.TagId,
				TagName = tag.TagName
			})
			.ToListAsync();

		return tags;
	}

	public async Task UpdateTagAsync(int tagId, UpdateTagRequest request)
	{
		var foundTag = await _context.Tags
			.Where(tag => tag.TagId == tagId)
			.SingleOrDefaultAsync();

		if (foundTag is null)
		{
			throw new ArgumentException($"Tag with id {tagId} was not found");
		}

		foundTag.TagName = request.TagName;
		await _context.SaveChangesAsync();
	}

	public async Task DeleteTagAsync(int tagId)
	{
		var foundTag = await _context.Tags
			.Where(tag => tag.TagId == tagId)
			.SingleOrDefaultAsync();

		if (foundTag is null)
		{
			throw new ArgumentException($"Tag with id {tagId} was not found");
		}

		_context.Tags.Remove(foundTag);

		await _context.SaveChangesAsync();
	}
}
