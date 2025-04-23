// <copyright file="Review.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks.Domain.Entities
{
    using System;

    /// <summary>
    /// Represents a review given by a user for a specific game.
    /// </summary>
    public class Review
    {
        private const int MinimumAllowedRating = 1;
        private const int MaximumAllowedRating = 5;

        private const string RatingOutOfRangeErrorMessage = "Rating must be between 1 and 5.";

        /// <summary>
        /// Initializes a new instance of the <see cref="Review"/> class with the specified reviewer, game, content, and rating.
        /// </summary>
        /// <param name="reviewer">The user who wrote the review.</param>
        /// <param name="game">The game being reviewed.</param>
        /// <param name="content">The content of the review.</param>
        /// <param name="rating">The rating given to the game.</param>
        public Review(User reviewer, Game game, string content, int rating)
        {
            this.Reviewer = reviewer;
            this.Game = game;
            this.Content = content;
            this.Rating = rating;
            this.ReviewDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Review"/> class.
        /// </summary>
        private Review()
        {
        }

        /// <summary>
        /// Gets the unique identifier of the review.
        /// </summary>
        public int ReviewId { get; private set; }

        /// <summary>
        /// Gets the user who wrote the review.
        /// </summary>
        public User Reviewer { get; private set; }

        /// <summary>
        /// Gets the game that was reviewed.
        /// </summary>
        public Game Game { get; private set; }

        /// <summary>
        /// Gets the content of the review.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets the rating given to the game.
        /// </summary>
        public int Rating { get; private set; }

        /// <summary>
        /// Gets the date and time when the review was created.
        /// </summary>
        public DateTime ReviewDate { get; private set; }

        /// <summary>
        /// Updates the content of the review.
        /// </summary>
        /// <param name="content">The new content for the review.</param>
        public void UpdateContent(string content)
        {
            this.Content = content;
        }

        /// <summary>
        /// Updates the rating of the review.
        /// </summary>
        /// <param name="rating">The new rating (must be between 1 and 5).</param>
        /// <exception cref="ArgumentException">Thrown when the rating is outside the range 1-5.</exception>
        public void UpdateRating(int rating)
        {
            if (rating < MinimumAllowedRating || rating > MaximumAllowedRating)
            {
                throw new ArgumentException(RatingOutOfRangeErrorMessage);
            }

            this.Rating = rating;
        }
    }
}