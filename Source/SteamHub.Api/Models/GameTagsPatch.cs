namespace SteamHub.Api.Models;

public class GameTagsPatch
{ 
    public GameTagsPatchType Type { get; set; }
    public ISet<int> TagIds { get; set; }
}

public enum GameTagsPatchType
{
    Insert,
    Delete
}