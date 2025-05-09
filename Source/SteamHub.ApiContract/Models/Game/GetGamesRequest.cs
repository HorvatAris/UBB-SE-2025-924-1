namespace SteamHub.ApiContract.Models.Game;
public class GetGamesRequest
{
    public GameStatusEnum? StatusIs { get; set; }

    public int? PublisherIdentifierIs { get; set; }

    public int? PublisherIdentifierIsnt { get; set; }
}