using BookLibrary.Api.Data;
using BookLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.GraphQL.DataLoaders;

/// <summary>
/// DataLoader for efficiently loading authors by their IDs.
/// Solves the N+1 query problem when fetching authors for multiple books.
/// </summary>
internal static class AuthorDataLoader
{
    /// <summary>
    /// Batch loads authors by their IDs.
    /// </summary>
    /// <param name="authorIds">The collection of author IDs to load.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary mapping author IDs to author entities.</returns>
    [DataLoader]
    public static async Task<Dictionary<int, Author>> GetAuthorByIdAsync(
        IReadOnlyList<int> authorIds,
        AppDbContext context,
        CancellationToken cancellationToken)
        => await context.Authors
            .Where(a => authorIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);
}

/// <summary>
/// DataLoader for efficiently loading books by author ID.
/// Returns multiple books per author (group DataLoader pattern).
/// </summary>
internal static class BooksByAuthorDataLoader
{
    /// <summary>
    /// Batch loads books grouped by author ID.
    /// </summary>
    /// <param name="authorIds">The collection of author IDs.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary mapping author IDs to arrays of books.</returns>
    [DataLoader]
    public static async Task<Dictionary<int, Book[]>> GetBooksByAuthorIdAsync(
        IReadOnlyList<int> authorIds,
        AppDbContext context,
        CancellationToken cancellationToken)
        => await context.Books
            .Where(b => authorIds.Contains(b.AuthorId))
            .GroupBy(b => b.AuthorId)
            .ToDictionaryAsync(g => g.Key, g => g.ToArray(), cancellationToken);
}
