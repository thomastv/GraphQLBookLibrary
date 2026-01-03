using BookLibrary.Api.Data;
using BookLibrary.Api.GraphQL.Types;
using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Exceptions;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for Book operations.
/// </summary>
[MutationType]
public static class BookMutations
{
    /// <summary>
    /// Adds a new book to the library.
    /// </summary>
    /// <param name="input">The book input data.</param>
    /// <param name="context">The database context.</param>
    /// <param name="eventSender">The subscription event sender.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created book.</returns>
    [Error(typeof(AuthorNotFoundException))]
    [Error(typeof(GenreNotFoundException))]
    [Error(typeof(DuplicateIsbnException))]
    public static async Task<Book> AddBookAsync(
        AddBookInput input,
        AppDbContext context,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        // Validate author exists
        var authorExists = await context.Authors.AnyAsync(a => a.Id == input.AuthorId, cancellationToken);
        if (!authorExists)
            throw new AuthorNotFoundException(input.AuthorId);

        // Check for duplicate ISBN
        if (!string.IsNullOrEmpty(input.Isbn))
        {
            var isbnExists = await context.Books.AnyAsync(b => b.Isbn == input.Isbn, cancellationToken);
            if (isbnExists)
                throw new DuplicateIsbnException(input.Isbn);
        }

        // Create book
        var book = new Book
        {
            Title = input.Title,
            AuthorId = input.AuthorId,
            Isbn = input.Isbn,
            Description = input.Description,
            PublishedDate = input.PublishedDate,
            PageCount = input.PageCount,
            Publisher = input.Publisher,
            Language = input.Language ?? "English"
        };

        // Add genres if provided
        if (input.GenreIds?.Count > 0)
        {
            var genres = await context.Genres
                .Where(g => input.GenreIds.Contains(g.Id))
                .ToListAsync(cancellationToken);

            if (genres.Count != input.GenreIds.Count)
            {
                var missingId = input.GenreIds.First(id => genres.All(g => g.Id != id));
                throw new GenreNotFoundException(missingId);
            }

            book.Genres = genres;
        }

        context.Books.Add(book);
        await context.SaveChangesAsync(cancellationToken);

        // Load author for subscription
        await context.Entry(book).Reference(b => b.Author).LoadAsync(cancellationToken);

        // Send subscription event
        await eventSender.SendAsync(nameof(Subscriptions.BookSubscriptions.OnBookAdded), book, cancellationToken);

        return book;
    }

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="input">The update input data.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated book.</returns>
    [Error(typeof(BookNotFoundException))]
    [Error(typeof(GenreNotFoundException))]
    [Error(typeof(DuplicateIsbnException))]
    public static async Task<Book> UpdateBookAsync(
        UpdateBookInput input,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var book = await context.Books
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.Id == input.Id, cancellationToken);

        if (book is null)
            throw new BookNotFoundException(input.Id);

        // Check for duplicate ISBN if changing
        if (!string.IsNullOrEmpty(input.Isbn) && input.Isbn != book.Isbn)
        {
            var isbnExists = await context.Books.AnyAsync(b => b.Isbn == input.Isbn && b.Id != input.Id, cancellationToken);
            if (isbnExists)
                throw new DuplicateIsbnException(input.Isbn);
        }

        // Update properties if provided
        if (input.Title is not null) book.Title = input.Title;
        if (input.Isbn is not null) book.Isbn = input.Isbn;
        if (input.Description is not null) book.Description = input.Description;
        if (input.PublishedDate.HasValue) book.PublishedDate = input.PublishedDate;
        if (input.PageCount.HasValue) book.PageCount = input.PageCount;
        if (input.Publisher is not null) book.Publisher = input.Publisher;
        if (input.Language is not null) book.Language = input.Language;

        // Update genres if provided
        if (input.GenreIds is not null)
        {
            var genres = await context.Genres
                .Where(g => input.GenreIds.Contains(g.Id))
                .ToListAsync(cancellationToken);

            if (genres.Count != input.GenreIds.Count)
            {
                var missingId = input.GenreIds.First(id => genres.All(g => g.Id != id));
                throw new GenreNotFoundException(missingId);
            }

            book.Genres.Clear();
            foreach (var genre in genres)
                book.Genres.Add(genre);
        }

        await context.SaveChangesAsync(cancellationToken);
        return book;
    }

    /// <summary>
    /// Deletes a book from the library.
    /// </summary>
    /// <param name="id">The book ID to delete.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully.</returns>
    [Error(typeof(BookNotFoundException))]
    public static async Task<bool> DeleteBookAsync(
        int id,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var book = await context.Books.FindAsync([id], cancellationToken);
        if (book is null)
            throw new BookNotFoundException(id);

        context.Books.Remove(book);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
