namespace SteamStore.Tests.Utils;

public class CreditCardProcessorTest
{
    private readonly CreditCardProcessor cardProcessor = new CreditCardProcessor();

    private const string CorrectCardNumber = "1234567891012";
    private const string CorrectExpirationDate = "12/30";
    private const string Whitespace = " ";
    private const string CorrectCvv = "999";
    private const string CorrectName = "TEST";

    [Fact]
    public async Task ProcessPaymentAsync_WithAllValidInputs_ReturnsTrue()
    {
        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, CorrectExpirationDate, CorrectCvv, CorrectName);

        Assert.True(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithWhitespaceCardNumber_ReturnsFalse()
    {
        var result = await cardProcessor.ProcessPaymentAsync(Whitespace, CorrectExpirationDate, CorrectCvv, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithShortCardNumber_ReturnsFalse()
    {
        string minimumCardNumberLengthNotMet = "123";

        var result = await cardProcessor.ProcessPaymentAsync(minimumCardNumberLengthNotMet, CorrectExpirationDate, CorrectCvv, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithLongCardNumber_ReturnsFalse()
    {
        string maximumCardNumberLengthExceeded = "12345678910121234567891012";

        var result = await cardProcessor.ProcessPaymentAsync(maximumCardNumberLengthExceeded, CorrectExpirationDate, CorrectCvv, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithWhitespaceExpirationDate_ReturnsFalse()
    {
        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, Whitespace, CorrectCvv, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithIncorrectFormatExpirationDate_ReturnsFalse()
    {
        string incorrectFormatExpirationDate = "222";

        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, incorrectFormatExpirationDate, CorrectCvv, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithWhitespaceCvv_ReturnsFalse()
    {
        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, CorrectExpirationDate, Whitespace, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithPastYearExpirationDate_ReturnsFalse()
    {
        string pastYearExpirationDate = "12/10";

        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, pastYearExpirationDate, CorrectCvv, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithPastMonthExpirationDate_ReturnsFalse()
    {
        string pastMonthExpirationDate = "1/25";

        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, pastMonthExpirationDate, CorrectCvv, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithIncorrectCvv_ReturnsFalse()
    {
        string incorrectCvv = "99999";

        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, CorrectExpirationDate, incorrectCvv, CorrectName);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithWhitespaceName_ReturnsFalse()
    {
        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, CorrectExpirationDate, CorrectCvv, Whitespace);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithInvalidName_ReturnsFalse()
    {
        string incorrectName = "123";

        var result = await cardProcessor.ProcessPaymentAsync(CorrectCardNumber, CorrectExpirationDate, CorrectCvv, incorrectName);

        Assert.False(result);
    }
}
