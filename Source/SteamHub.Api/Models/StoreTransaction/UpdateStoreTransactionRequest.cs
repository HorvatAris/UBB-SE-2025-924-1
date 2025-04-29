namespace SteamHub.Api.Models.StoreTransaction;

public class UpdateStoreTransactionRequest
{
    public DateTime Date { get; set; }

    public float Amount { get; set; }

    public bool WithMoney { get; set; }
}
