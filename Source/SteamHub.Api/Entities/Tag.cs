namespace SteamHub.Api.Entities;

public class Tag
{
	public int TagId { get; set; }

	public string TagName { get; set; }

    public List<Game> Games { get; set; }
}
