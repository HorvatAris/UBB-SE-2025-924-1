namespace SteamHub.Api.Context;

using Entities;
using Microsoft.EntityFrameworkCore;
using Models;

public class TagRepository : ITagRepository
{
	private readonly DataContext context;

	public TagRepository(DataContext context)
	{
		this.context = context;
	}

	public async Task<CreateTagResponse> CreateTagAsync(CreateTagRequest request)
	{
		var isDuplicate = await context.Tags
			.AnyAsync(tag => tag.TagName == request.TagName);

		if (isDuplicate)
		{
			throw new ArgumentException($"Tag with name {request.TagName} already exists");
		}

		var newTag = new Tag
		{
			TagName = request.TagName
		};

		context.Add(newTag);

		await context.SaveChangesAsync();

		return new CreateTagResponse
		{
			TagId = newTag.TagId
		};
	}

	public async Task<TagNameOnlyResponse?> GetTagByIdAsync(int tagId)
	{
		var foundTag = await context.Tags
			.Where(tag => tag.TagId == tagId)
			.Select(tag => new TagNameOnlyResponse
			{
				TagName = tag.TagName
			})
			.SingleOrDefaultAsync();

		return foundTag;
	}

	public async Task<GetTagsResponse> GetAllTagsAsync()
	{
		var tags = await context.Tags
			.Select(tag => new TagSummaryResponse
			{
				TagId = tag.TagId,
				TagName = tag.TagName
			})
			.ToListAsync();

		return new GetTagsResponse
		{
			Tags = tags
		};
	}

	public async Task UpdateTagAsync(int tagId, UpdateTagRequest request)
	{
		var foundTag = await context.Tags
			.Where(tag => tag.TagId == tagId)
			.SingleOrDefaultAsync();

		if (foundTag is null)
		{
			throw new ArgumentException($"Tag with id {tagId} was not found");
		}

		foundTag.TagName = request.TagName;
		await context.SaveChangesAsync();
	}

	public async Task DeleteTagAsync(int tagId)
	{
		var foundTag = await context.Tags
			.Where(tag => tag.TagId == tagId)
			.SingleOrDefaultAsync();

		if (foundTag is null)
		{
			throw new ArgumentException($"Tag with id {tagId} was not found");
		}

		context.Tags.Remove(foundTag);

		await context.SaveChangesAsync();
	}
}
