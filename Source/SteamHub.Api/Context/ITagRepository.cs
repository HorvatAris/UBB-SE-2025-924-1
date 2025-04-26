using SteamHub.Api.Models;

namespace SteamHub.Api.Context;

public interface ITagRepository
{
	Task<CreateTagResponse> CreateTagAsync(CreateTagRequest request);
	Task DeleteTagAsync(int tagId);
	Task<GetTagsResponse> GetAllTagsAsync();
	Task<TagNameOnlyResponse?> GetTagByIdAsync(int tagId);
	Task UpdateTagAsync(int tagId, UpdateTagRequest request);
}