// <copyright file="PaypalProcessor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class PaypalProcessor
{
    private const int DelayTimePayment = 2000;
    private const int MinimumPasswordLength = 8;
    private const string ValidEmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public async Task<bool> ProcessPaymentAsync(string email, string password, decimal amount)
    {
        if (this.IsValidEmail(email) && this.IsValidPassword(password))
        {
            // Simulate a successful payment
            await Task.Delay(DelayTimePayment);
            return true;
        }

        return false;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return Regex.IsMatch(email, ValidEmailPattern);
    }

    private bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length > MinimumPasswordLength;
    }
}