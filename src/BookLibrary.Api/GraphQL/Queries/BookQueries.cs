using BookLibrary.Api.Data;
using BookLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for Book operations.
/// </summary>
[QueryType]
public static class BookQueries
{
    /// <summary>
    /// Gets all books with optional filtering, sorting, and projection.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of books.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Book> GetBooks(AppDbContext context)
        => context.Books;

    /// <summary>
    /// Gets a single book by its ID.
    /// </summary>
    /// <param name="id">The book ID.</param>
    /// <param name="context">The database context.</param>
    /// <returns>The book if found, otherwise null.</returns>
    [UseProjection]
    public static async Task<Book?> GetBookAsync(
        int id,
        AppDbContext context)
        => await context.Books
            .Include(b => b.Author)
            .Include(b => b.Genres)
            .Include(b => b.Reviews)
            .FirstOrDefaultAsync(b => b.Id == id);

    /// <summary>
    /// Gets books by author ID.
    /// </summary>
    /// <param name="authorId">The author's ID.</param>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of books by the specified author.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Book> GetBooksByAuthor(
        int authorId,
        AppDbContext context)
        => context.Books.Where(b => b.AuthorId == authorId);

    /// <summary>
    /// Gets the top-rated books.
    /// </summary>
    /// <param name="count">Number of books to return (default: 10).</param>
    /// <param name="context">The database context.</param>
    /// <returns>A collection of top-rated books.</returns>
    [UseProjection]
    public static async Task<IEnumerable<Book>> GetTopRatedBooksAsync(
        int count,
        AppDbContext context)
        => await context.Books
            .Include(b => b.Reviews)
            .Include(b => b.Author)
            .Where(b => b.Reviews.Count > 0)
            .OrderByDescending(b => b.Reviews.Average(r => r.Rating))
            .Take(count > 0 ? count : 10)
            .ToListAsync();

    /// <summary>
    /// Searches books by title.
    /// </summary>
    /// <param name="searchTerm">The search term to match against book titles.</param>
    /// <param name="context">The database context.</param>
    /// <returns>A queryable collection of matching books.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Book> SearchBooks(
        string searchTerm,
        AppDbContext context)
        => context.Books.Where(b => 
            b.Title.Contains(searchTerm) || 
            (b.Description != null && b.Description.Contains(searchTerm)));
}
