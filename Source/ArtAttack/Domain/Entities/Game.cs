// <copyright file="Game.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Domain.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the possible statuses a game can have.
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// The game is currently available for users.
        /// </summary>
        Available,

        /// <summary>
        /// The game is currently unavailable.
        /// </summary>
        Unavailable,

        /// <summary>
        /// The game is currently in development and not released.
        /// </summary>
        InDevelopment,

        /// <summary>
        /// The game has been discontinued.
        /// </summary>
        Discontinued,
    }

    /// <summary>
    /// Represents a video game with its properties and reviews.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Represents the default status assigned to a game when it is created.
        /// </summary>
        private const string DefaultStatus = "Available";

        /// <summary>
        /// Represents the price threshold to decide if a price should be displayed.
        /// </summary>
        private const float MinimumDisplayPrice = 0.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="title">The title of the game.</param>
        /// <param name="price">The price of the game.</param>
        /// <param name="genre">The genre of the game.</param>
        /// <param name="description">The description of the game.</param>
        public Game(string title, float price, string genre, string description)
        {
            this.GameTitle = title;
            this.Price = price;
            this.Genre = genre;
            this.GameDescription = description;
            this.GameReviews = new List<Review>();
            this.Status = DefaultStatus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class for EF Core.
        /// </summary>
        private Game()
        {
            this.GameTitle = string.Empty;
            this.Genre = string.Empty;
            this.GameDescription = string.Empty;
            this.GameReviews = new List<Review>();
        }

        /// <summary>
        /// Gets or sets the internal identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the internal name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the unique numeric identifier for the game.
        /// </summary>
        public int GameId { get; private set; }

        /// <summary>
        /// Gets the title of the game.
        /// </summary>
        public string GameTitle { get; private set; }

        /// <summary>
        /// Gets the price of the game.
        /// </summary>
        public float Price { get; private set; }

        /// <summary>
        /// Gets the genre of the game.
        /// </summary>
        public string Genre { get; private set; }

        /// <summary>
        /// Gets the description of the game.
        /// </summary>
        public string GameDescription { get; private set; }

        /// <summary>
        /// Gets the list of reviews associated with the game.
        /// </summary>
        public ICollection<Review> GameReviews { get; private set; }

        /// <summary>
        /// Gets the status of the game.
        /// </summary>
        public string Status { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the recommended system specifications for the game.
        /// </summary>
        public float RecommendedSpecifications { get; private set; }

        /// <summary>
        /// Gets the minimum system specifications required to run the game.
        /// </summary>
        public float MinimumSpecifications { get; private set; }

        /// <summary>
        /// Sets the unique identifier of the game.
        /// </summary>
        /// <param name="id">The identifier of the game.</param>
        public void SetGameId(int id)
        {
            this.GameId = id;
        }

        /// <summary>
        /// Gets the title of the game.
        /// </summary>
        /// <returns>The title of the game.</returns>
        public string GetTitle()
        {
            return this.GameTitle;
        }

        /// <summary>
        /// Sets the title of the game.
        /// </summary>
        /// <param name="title">The title of the game.</param>
        public void SetTitle(string title)
        {
            this.GameTitle = title;
        }

        /// <summary>
        /// Gets the price of the game.
        /// </summary>
        /// <returns>The price of the game.</returns>
        public float GetPrice()
        {
            return this.Price;
        }

        /// <summary>
        /// Sets the price of the game.
        /// </summary>
        /// <param name="price">The price of the game.</param>
        public void SetPrice(float price)
        {
            this.Price = price;
        }

        /// <summary>
        /// Gets the genre of the game.
        /// </summary>
        /// <returns>The genre of the game.</returns>
        public string GetGenre()
        {
            return this.Genre;
        }

        /// <summary>
        /// Sets the genre of the game.
        /// </summary>
        /// <param name="genre">The genre of the game.</param>
        public void SetGenre(string genre)
        {
            this.Genre = genre;
        }

        /// <summary>
        /// Gets the description of the game.
        /// </summary>
        /// <returns>The description of the game.</returns>
        public string GetDescription()
        {
            return this.GameDescription;
        }

        /// <summary>
        /// Sets the description of the game.
        /// </summary>
        /// <param name="description">The description of the game.</param>
        public void SetDescription(string description)
        {
            this.GameDescription = description;
        }

        /// <summary>
        /// Gets the current status of the game.
        /// </summary>
        /// <returns>The status of the game.</returns>
        public string GetStatus()
        {
            return this.Status;
        }

        /// <summary>
        /// Sets the current status of the game.
        /// </summary>
        /// <param name="status">The status of the game.</param>
        public void SetStatus(string status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Adds a review to the game.
        /// </summary>
        /// <param name="review">The review of the game.</param>
        public void AddReview(Review review)
        {
            this.GameReviews.Add(review);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Price > MinimumDisplayPrice ? $"{this.GameTitle} (${this.Price:F2})" : this.GameTitle;
        }
    }
}