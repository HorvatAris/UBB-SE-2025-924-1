using SteamHub.Api.Entities;

namespace SteamHub.Api.Context;

public class GamesQueryParams
{
    public GameStatus? StatusIs { get; set; }
    public int? PublisherIdentifierIs { get; set; }
    public int? PublisherIdentifierIsnt { get; set; }
}