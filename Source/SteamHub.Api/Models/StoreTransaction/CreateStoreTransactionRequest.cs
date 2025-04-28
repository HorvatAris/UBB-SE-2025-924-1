namespace SteamHub.Api.Models.StoreTransaction;

public class CreateStoreTransactionRequest
{
    public int UserId { get; set; }

    public int GameId { get; set; }

    public DateTime Date { get; set; }

    public float Amount { get; set; }

    public bool WithMoney { get; set; }
}
