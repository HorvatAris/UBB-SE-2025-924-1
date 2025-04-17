namespace SteamStore.Tests.Utils;

public class PaypalProcessorTest
{
    private readonly PaypalProcessor paypalProcessor = new PaypalProcessor();
    private const string CORRECT_EMAIL = "test@test.com";
    private const string INCORRECT_EMAIL = "*&^$%$^";
    private const string CORRECT_PASSWORD = "123456789";
    private const string INCORRECT_PASSWORD = "123";
    private const string WHITESPACE = " ";
    private const decimal AMOUNT = -1m;
    public static IEnumerable<object[]> PaymentData =>
        new List<object[]>
        {
            new object[] { true, CORRECT_EMAIL, CORRECT_PASSWORD, AMOUNT },
            new object[] { false, INCORRECT_EMAIL, CORRECT_PASSWORD, AMOUNT },
            new object[] { false, WHITESPACE, CORRECT_PASSWORD, AMOUNT },
            new object[] { false, CORRECT_EMAIL, WHITESPACE, AMOUNT },
            new object[] { false, CORRECT_EMAIL, INCORRECT_PASSWORD, AMOUNT }
        };

    [Theory]
    [MemberData(nameof(PaymentData))]
    public async Task ProcessPaymentAsync(bool result, string email, string password, decimal amount)
    {
        Assert.Equal(result, await paypalProcessor.ProcessPaymentAsync(email, password, amount));
    }
}