namespace SteamHub.Api.Models;

public class PatchGameTagsRequest
{
    public GameTagsPatchType Type { get; set; }

    public ISet<int> TagIds { get; set; }
}