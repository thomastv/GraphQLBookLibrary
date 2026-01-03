using BookLibrary.Api.Data;
using BookLibrary.Domain.Entities;

namespace BookLibrary.Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for Genre operations.
/// </summary>
[QueryType]
public static class GenreQueries
{
    /// <summary>
    /// Gets all genres with optional filtering, sorting, and projection.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of genres.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Genre> GetGenres(AppDbContext context)
        => context.Genres;

    /// <summary>
    /// Gets a single genre by its ID.
    /// </summary>
    /// <param name="id">The genre ID.</param>
    /// <param name="context">The database context.</param>
    /// <returns>The genre if found, otherwise null.</returns>
    [UseProjection]
    public static async Task<Genre?> GetGenreAsync(
        int id,
        AppDbContext context,
        CancellationToken cancellationToken)
        => await context.Genres.FindAsync([id], cancellationToken);
}
