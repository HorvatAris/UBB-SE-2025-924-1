using SteamHub.Api.Models.Tag;

namespace SteamHub.Api.Context.Repositories;

public interface ITagRepository
{
	Task<CreateTagResponse> CreateTagAsync(CreateTagRequest request);
	Task DeleteTagAsync(int tagId);
	Task<GetTagsResponse> GetAllTagsAsync();
	Task<TagNameOnlyResponse?> GetTagByIdAsync(int tagId);
	Task UpdateTagAsync(int tagId, UpdateTagRequest request);
}