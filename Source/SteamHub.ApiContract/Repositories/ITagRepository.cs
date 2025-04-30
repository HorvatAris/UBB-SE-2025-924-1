namespace SteamHub.ApiContract.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Tag;

    public interface ITagRepository
    {
        Task<CreateTagResponse> CreateTagAsync(CreateTagRequest request);
        Task DeleteTagAsync(int tagId);
        Task<GetTagsResponse> GetAllTagsAsync();
        Task<TagNameOnlyResponse?> GetTagByIdAsync(int tagId);
        Task UpdateTagAsync(int tagId, UpdateTagRequest request);
    }
}
