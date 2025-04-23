// <copyright file="User.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Domain.Entities
{
    using System;

    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User
    {
        private const float InitialWalletBalance = 0;
        private const float InitialPointBalance = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with the specified username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        public User(string username)
        {
            this.Username = username;
            this.WalletBalance = InitialWalletBalance;
            this.PointBalance = InitialPointBalance;
            this.IsDeveloper = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        private User()
        {
        }

        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the wallet balance of the user.
        /// </summary>
        public float WalletBalance { get; private set; }

        /// <summary>
        /// Gets the point balance of the user.
        /// </summary>
        public float PointBalance { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is a developer.
        /// </summary>
        public bool IsDeveloper { get; private set; }

        /// <summary>
        /// Sets the user's unique identifier.
        /// </summary>
        /// <param name="id">The user ID.</param>
        public void SetUserId(int id)
        {
            this.UserId = id;
        }

        /// <summary>
        /// Sets the user's username.
        /// </summary>
        /// <param name="username">The new username.</param>
        public void SetUsername(string username)
        {
            this.Username = username;
        }

        /// <summary>
        /// Gets the user's wallet balance.
        /// </summary>
        /// <returns>The wallet balance.</returns>
        public float GetWalletBalance()
        {
            return this.WalletBalance;
        }

        /// <summary>
        /// Gets the user's point balance.
        /// </summary>
        /// <returns>The point balance.</returns>
        public float GetPointBalance()
        {
            return this.PointBalance;
        }

        /// <summary>
        /// Sets the user's wallet balance.
        /// </summary>
        /// <param name="balance">The new wallet balance.</param>
        public void SetWalletBalance(float balance)
        {
            this.WalletBalance = balance;
        }

        /// <summary>
        /// Sets the user's point balance.
        /// </summary>
        /// <param name="balance">The new point balance.</param>
        public void SetPointBalance(float balance)
        {
            this.PointBalance = balance;
        }

        /// <summary>
        /// Sets whether the user is a developer.
        /// </summary>
        /// <param name="state">True if the user is a developer; otherwise, false.</param>
        public void SetIsDeveloper(bool state)
        {
            this.IsDeveloper = state;
        }
    }
}