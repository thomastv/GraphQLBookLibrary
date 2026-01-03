using BookLibrary.Api.Data;
using BookLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.GraphQL.DataLoaders;

/// <summary>
/// DataLoader for efficiently loading reviews by book ID.
/// Returns multiple reviews per book (group DataLoader pattern).
/// </summary>
internal static class ReviewDataLoader
{
    /// <summary>
    /// Batch loads reviews grouped by book ID.
    /// </summary>
    /// <param name="bookIds">The collection of book IDs.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary mapping book IDs to arrays of reviews.</returns>
    [DataLoader]
    public static async Task<Dictionary<int, Review[]>> GetReviewsByBookIdAsync(
        IReadOnlyList<int> bookIds,
        AppDbContext context,
        CancellationToken cancellationToken)
        => await context.Reviews
            .Where(r => bookIds.Contains(r.BookId))
            .GroupBy(r => r.BookId)
            .ToDictionaryAsync(g => g.Key, g => g.ToArray(), cancellationToken);
}

/// <summary>
/// DataLoader for efficiently loading a single book by ID.
/// </summary>
internal static class BookDataLoader
{
    /// <summary>
    /// Batch loads books by their IDs.
    /// </summary>
    /// <param name="bookIds">The collection of book IDs to load.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary mapping book IDs to book entities.</returns>
    [DataLoader]
    public static async Task<Dictionary<int, Book>> GetBookByIdAsync(
        IReadOnlyList<int> bookIds,
        AppDbContext context,
        CancellationToken cancellationToken)
        => await context.Books
            .Where(b => bookIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, cancellationToken);
}
