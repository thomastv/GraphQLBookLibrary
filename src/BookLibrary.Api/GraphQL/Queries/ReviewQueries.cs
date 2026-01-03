using BookLibrary.Api.Data;
using BookLibrary.Domain.Entities;

namespace BookLibrary.Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for Review operations.
/// </summary>
[QueryType]
public static class ReviewQueries
{
    /// <summary>
    /// Gets all reviews with optional filtering, sorting, and projection.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of reviews.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Review> GetReviews(AppDbContext context)
        => context.Reviews;

    /// <summary>
    /// Gets reviews for a specific book.
    /// </summary>
    /// <param name="bookId">The book ID.</param>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of reviews for the specified book.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Review> GetReviewsByBook(
        int bookId,
        AppDbContext context)
        => context.Reviews.Where(r => r.BookId == bookId);

    /// <summary>
    /// Gets reviews by a specific reviewer.
    /// </summary>
    /// <param name="reviewerName">The reviewer's name.</param>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of reviews by the specified reviewer.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Review> GetReviewsByReviewer(
        string reviewerName,
        AppDbContext context)
        => context.Reviews.Where(r => r.ReviewerName.Contains(reviewerName));

    /// <summary>
    /// Gets reviews with a specific rating.
    /// </summary>
    /// <param name="rating">The rating to filter by (1-5).</param>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of reviews with the specified rating.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Review> GetReviewsByRating(
        int rating,
        AppDbContext context)
        => context.Reviews.Where(r => r.Rating == rating);
}
