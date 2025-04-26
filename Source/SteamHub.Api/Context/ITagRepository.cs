using SteamHub.Api.Models;

namespace SteamHub.Api.Context;

public interface ITagRepository
{
	Task CreateTagAsync(CreateTagRequest request);
	Task DeleteTagAsync(int tagId);
	Task<List<TagDetailedResponse>> GetAllTagsAsync();
	Task<TagResponse?> GetTagByIdAsync(int tagId);
	Task UpdateTagAsync(int tagId, UpdateTagRequest request);
}