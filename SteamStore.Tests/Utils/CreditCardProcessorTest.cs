namespace SteamStore.Tests.Utils;

public class CreditCardProcessorTest
{
    private readonly CreditCardProcessor cardProcessor = new CreditCardProcessor();
    private const string CORRECT_CARD_NUMBER = "1234567891012";
    private const string MAX_CARD_NUMBER_LENGTH_EXCEEDED = "12345678910121234567891012";
    private const string MIN_CARD_NUMBER_LENGTH_NOT_MET = "123";
    private const string WHITESPACE = " ";
    private const string CORRECT_EXPIRATION_DATE = "12/30";
    private const string INCORRECT_FORMAT_EXPIRATION_DATE = "222";
    private const string PAST_YEAR_EXPIRATION_DATE = "12/10";
    private const string PAST_MONTH_EXPIRATION_DATE = "1/25";
    private const string CORRECT_CVV = "999";
    private const string INCORRECT_CVV = "99999";
    private const string CORRECT_NAME = "TEST";
    private const string INCORRECT_NAME = "123";

    [Theory]
    [InlineData(true, CORRECT_CARD_NUMBER, CORRECT_EXPIRATION_DATE, CORRECT_CVV, CORRECT_NAME)]
    [InlineData(false, WHITESPACE, CORRECT_EXPIRATION_DATE, CORRECT_CVV, CORRECT_NAME)]
    [InlineData(false, MIN_CARD_NUMBER_LENGTH_NOT_MET, CORRECT_EXPIRATION_DATE, CORRECT_CVV, CORRECT_NAME)]
    [InlineData(false, MAX_CARD_NUMBER_LENGTH_EXCEEDED, CORRECT_EXPIRATION_DATE, CORRECT_CVV, CORRECT_NAME)]
    [InlineData(false, CORRECT_CARD_NUMBER, WHITESPACE, CORRECT_CVV, CORRECT_NAME)]
    [InlineData(false, CORRECT_CARD_NUMBER, INCORRECT_FORMAT_EXPIRATION_DATE, CORRECT_CVV, CORRECT_NAME)]
    [InlineData(false, CORRECT_CARD_NUMBER, CORRECT_EXPIRATION_DATE, WHITESPACE, CORRECT_NAME)]
    [InlineData(false, CORRECT_CARD_NUMBER, PAST_YEAR_EXPIRATION_DATE, WHITESPACE, CORRECT_NAME)]
    [InlineData(false, CORRECT_CARD_NUMBER, PAST_MONTH_EXPIRATION_DATE, WHITESPACE, CORRECT_NAME)]
    [InlineData(false, CORRECT_CARD_NUMBER, CORRECT_EXPIRATION_DATE, INCORRECT_CVV, CORRECT_NAME)]
    [InlineData(false, CORRECT_CARD_NUMBER, CORRECT_EXPIRATION_DATE, CORRECT_CVV, WHITESPACE)]
    [InlineData(false, CORRECT_CARD_NUMBER, CORRECT_EXPIRATION_DATE, CORRECT_CVV, INCORRECT_NAME)]
    public async Task ProcessPaymentAsync(bool result, string cardNumber, string expirationDate, string cvv, string ownerName)
    {
        Assert.Equal(result, await cardProcessor.ProcessPaymentAsync(cardNumber, expirationDate, cvv, ownerName));
    }
}