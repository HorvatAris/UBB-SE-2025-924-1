namespace SteamStore.Tests.Utils;

public class PaypalProcessorTest
{
    private const string ValidEmail = "test@test.com";
    private const string ValidPassword = "123456789";
    private const string Whitespace = " ";
    private const decimal PaymentAmount = 1m;

    private readonly PaypalProcessor paypalProcessor = new PaypalProcessor();

    [Fact]
    public async Task ProcessPaymentAsync_WithValidEmailAndPassword_ReturnsTrue()
    {
        bool result = await paypalProcessor.ProcessPaymentAsync(ValidEmail, ValidPassword, PaymentAmount);

        Assert.True(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithInvalidEmail_ReturnsFalse()
    {
        string invalidEmail = "*&^$%$^";

        bool result = await paypalProcessor.ProcessPaymentAsync(invalidEmail, ValidPassword, PaymentAmount);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithWhitespaceEmail_ReturnsFalse()
    {
        bool result = await paypalProcessor.ProcessPaymentAsync(Whitespace, ValidPassword, PaymentAmount);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithWhitespacePassword_ReturnsFalse()
    {
        bool result = await paypalProcessor.ProcessPaymentAsync(ValidEmail, Whitespace, PaymentAmount);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithInvalidPassword_ReturnsFalse()
    {
        string invalidPassword = "123";

        bool result = await paypalProcessor.ProcessPaymentAsync(ValidEmail, invalidPassword, PaymentAmount);

        Assert.False(result);
    }
}
