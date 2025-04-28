using SteamHub.ApiContract.Models.Game;

namespace SteamHub.Api.Entities;

public class GameStatus
{
    public GameStatusEnum Id { get; set; }
    public string Name { get; set; }
}