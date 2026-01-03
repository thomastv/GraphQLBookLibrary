using BookLibrary.Api.Data;
using BookLibrary.Api.GraphQL.Types;
using BookLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for Genre operations.
/// </summary>
[MutationType]
public static class GenreMutations
{
    /// <summary>
    /// Adds a new genre.
    /// </summary>
    /// <param name="input">The genre input data.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created genre.</returns>
    public static async Task<Genre> AddGenreAsync(
        AddGenreInput input,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var genre = new Genre
        {
            Name = input.Name,
            Description = input.Description
        };

        context.Genres.Add(genre);
        await context.SaveChangesAsync(cancellationToken);

        return genre;
    }

    /// <summary>
    /// Deletes a genre.
    /// </summary>
    /// <param name="id">The genre ID to delete.</param>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully.</returns>
    public static async Task<bool> DeleteGenreAsync(
        int id,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var genre = await context.Genres.FindAsync([id], cancellationToken);
        if (genre is null)
            return false;

        context.Genres.Remove(genre);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
