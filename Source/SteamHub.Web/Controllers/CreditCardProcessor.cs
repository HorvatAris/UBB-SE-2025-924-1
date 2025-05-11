// <copyright file="CreditCardProcessor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class CreditCardProcessor
{
    private const int DelayForPayment = 200;
    private const int MinimumLenthCardNumber = 13;
    private const int MaximumLengthCardNumber = 19;
    private const string ValidExpirationDatePattern = @"^\d{2}/\d{2}$";
    private const char Separator = '/';
    private const string ReplaceCardNumberRegex = @"[^\d]";
    private const int FirstPartOfExpirationDateIndex = 0;
    private const int SecondPartOfExpirationDateIndex = 1;
    private const string ValidCVVPattern = @"^\d{3,4}$";
    private const int GetLastTwoDigits = 100;
    private const string ValidOwnerNamePattern = @"^[a-zA-Z\s]+$";

    public async Task<bool> ProcessPaymentAsync(string cardNumber, string expirationDate, string cvv, string ownerName)
    {
        if (!IsValidCardNumber(cardNumber) ||
            !IsValidExpirationDate(expirationDate) ||
            !IsValidCvv(cvv) ||
            !IsValidOwnerName(ownerName))
        {
            return false;
        }

        await Task.Delay(DelayForPayment);
        return true;
    }

    private static bool IsValidCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return false;
        }

        cardNumber = Regex.Replace(cardNumber, ReplaceCardNumberRegex, string.Empty);

        return cardNumber.Length is >= MinimumLenthCardNumber and <= MaximumLengthCardNumber;
    }

    private static bool IsValidExpirationDate(string expirationDate)
    {
        if (string.IsNullOrWhiteSpace(expirationDate))
        {
            return false;
        }

        if (!Regex.IsMatch(expirationDate, ValidExpirationDatePattern))
        {
            return false;
        }

        var parts = expirationDate.Split(Separator);
        var month = int.Parse(parts[FirstPartOfExpirationDateIndex]);
        var year = int.Parse(parts[SecondPartOfExpirationDateIndex]);

        var currentYear = DateTime.Now.Year % GetLastTwoDigits;
        var currentMonth = DateTime.Now.Month;

        return year >= currentYear && (year != currentYear || month >= currentMonth);
    }

    private static bool IsValidCvv(string cvv)
    {
        return !string.IsNullOrWhiteSpace(cvv) && Regex.IsMatch(cvv, ValidCVVPattern);
    }

    private static bool IsValidOwnerName(string ownerName)
    {
        return !string.IsNullOrWhiteSpace(ownerName) && Regex.IsMatch(ownerName, ValidOwnerNamePattern);
    }
}