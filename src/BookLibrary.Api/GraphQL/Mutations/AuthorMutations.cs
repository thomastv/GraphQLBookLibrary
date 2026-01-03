using BookLibrary.Api.Data;
using BookLibrary.Api.GraphQL.Types;
using BookLibrary.Domain.Entities;
using BookLibrary.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for Author operations.
/// </summary>
[MutationType]
public static class AuthorMutations
{
    /// <summary>
    /// Adds a new author.
    /// </summary>
    /// <param name="input">The author input data.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created author.</returns>
    public static async Task<Author> AddAuthorAsync(
        AddAuthorInput input,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var author = new Author
        {
            Name = input.Name,
            Biography = input.Biography,
            DateOfBirth = input.DateOfBirth,
            Nationality = input.Nationality,
            ImageUrl = input.ImageUrl
        };

        context.Authors.Add(author);
        await context.SaveChangesAsync(cancellationToken);

        return author;
    }

    /// <summary>
    /// Updates an existing author.
    /// </summary>
    /// <param name="input">The update input data.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated author.</returns>
    [Error(typeof(AuthorNotFoundException))]
    public static async Task<Author> UpdateAuthorAsync(
        UpdateAuthorInput input,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var author = await context.Authors.FindAsync([input.Id], cancellationToken);
        if (author is null)
            throw new AuthorNotFoundException(input.Id);

        // Update properties if provided
        if (input.Name is not null) author.Name = input.Name;
        if (input.Biography is not null) author.Biography = input.Biography;
        if (input.DateOfBirth.HasValue) author.DateOfBirth = input.DateOfBirth;
        if (input.Nationality is not null) author.Nationality = input.Nationality;
        if (input.ImageUrl is not null) author.ImageUrl = input.ImageUrl;

        await context.SaveChangesAsync(cancellationToken);
        return author;
    }

    /// <summary>
    /// Deletes an author and all their books.
    /// </summary>
    /// <param name="id">The author ID to delete.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully.</returns>
    [Error(typeof(AuthorNotFoundException))]
    public static async Task<bool> DeleteAuthorAsync(
        int id,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var author = await context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (author is null)
            throw new AuthorNotFoundException(id);

        context.Authors.Remove(author);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
