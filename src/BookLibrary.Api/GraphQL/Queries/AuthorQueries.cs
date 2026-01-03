using BookLibrary.Api.Data;
using BookLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for Author operations.
/// </summary>
[QueryType]
public static class AuthorQueries
{
    /// <summary>
    /// Gets all authors with optional filtering, sorting, and projection.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of authors.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Author> GetAuthors(AppDbContext context)
        => context.Authors;

    /// <summary>
    /// Gets a single author by their ID.
    /// </summary>
    /// <param name="id">The author ID.</param>
    /// <param name="context">The database context.</param>
    /// <returns>The author if found, otherwise null.</returns>
    [UseProjection]
    public static async Task<Author?> GetAuthorAsync(
        int id,
        AppDbContext context)
        => await context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

    /// <summary>
    /// Gets authors by nationality.
    /// </summary>
    /// <param name="nationality">The nationality to filter by.</param>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of authors from the specified nationality.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Author> GetAuthorsByNationality(
        string nationality,
        AppDbContext context)
        => context.Authors.Where(a => a.Nationality == nationality);

    /// <summary>
    /// Searches authors by name.
    /// </summary>
    /// <param name="searchTerm">The search term to match against author names.</param>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of matching authors.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Author> SearchAuthors(
        string searchTerm,
        AppDbContext context)
        => context.Authors.Where(a => a.Name.Contains(searchTerm));
}
