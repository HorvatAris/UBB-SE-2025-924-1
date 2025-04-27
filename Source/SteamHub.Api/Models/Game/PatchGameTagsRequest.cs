namespace SteamHub.Api.Models.Game;

public class PatchGameTagsRequest
{
    public GameTagsPatchType Type { get; set; }

    public ISet<int> TagIds { get; set; }
}