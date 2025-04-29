namespace SteamHub.Api.Models.StoreTransaction;

public class StoreTransactionResponse
{
    public int StoreTransactionId { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public DateTime Date { get; set; }

    public float Amount { get; set; }

    public bool WithMoney { get; set; }
}
