using BookLibrary.Api.Data;
using BookLibrary.Api.GraphQL.Types;
using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Exceptions;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for Review operations.
/// </summary>
[MutationType]
public static class ReviewMutations
{
    /// <summary>
    /// Adds a new review for a book.
    /// </summary>
    /// <param name="input">The review input data.</param>
    /// <param name="context">The database context.</param>
    /// <param name="eventSender">The subscription event sender.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created review.</returns>
    [Error(typeof(BookNotFoundException))]
    [Error(typeof(InvalidRatingException))]
    public static async Task<Review> AddReviewAsync(
        AddReviewInput input,
        AppDbContext context,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        // Validate rating
        if (input.Rating < Review.MinRating || input.Rating > Review.MaxRating)
            throw new InvalidRatingException(input.Rating);

        // Validate book exists
        var bookExists = await context.Books.AnyAsync(b => b.Id == input.BookId, cancellationToken);
        if (!bookExists)
            throw new BookNotFoundException(input.BookId);

        var review = new Review
        {
            BookId = input.BookId,
            Rating = input.Rating,
            ReviewerName = input.ReviewerName,
            Comment = input.Comment,
            ReviewerEmail = input.ReviewerEmail,
            CreatedAt = DateTime.UtcNow
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync(cancellationToken);

        // Load book for subscription
        await context.Entry(review).Reference(r => r.Book).LoadAsync(cancellationToken);

        // Send subscription event
        await eventSender.SendAsync(nameof(Subscriptions.BookSubscriptions.OnReviewPosted), review, cancellationToken);

        return review;
    }

    /// <summary>
    /// Updates an existing review.
    /// </summary>
    /// <param name="input">The update input data.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated review.</returns>
    [Error(typeof(ReviewNotFoundException))]
    [Error(typeof(InvalidRatingException))]
    public static async Task<Review> UpdateReviewAsync(
        UpdateReviewInput input,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var review = await context.Reviews.FindAsync([input.Id], cancellationToken);
        if (review is null)
            throw new ReviewNotFoundException(input.Id);

        // Validate rating if provided
        if (input.Rating.HasValue)
        {
            if (input.Rating < Review.MinRating || input.Rating > Review.MaxRating)
                throw new InvalidRatingException(input.Rating.Value);
            review.Rating = input.Rating.Value;
        }

        if (input.Comment is not null)
            review.Comment = input.Comment;

        review.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return review;
    }

    /// <summary>
    /// Deletes a review.
    /// </summary>
    /// <param name="id">The review ID to delete.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully.</returns>
    [Error(typeof(ReviewNotFoundException))]
    public static async Task<bool> DeleteReviewAsync(
        int id,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var review = await context.Reviews.FindAsync([id], cancellationToken);
        if (review is null)
            throw new ReviewNotFoundException(id);

        context.Reviews.Remove(review);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
