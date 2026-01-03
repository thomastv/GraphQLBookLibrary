namespace BookLibrary.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-specific exceptions.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Creates a new instance of <see cref="DomainException"/>.
    /// </summary>
    /// <param name="message">The error message.</param>
    protected DomainException(string message) : base(message) { }

    /// <summary>
    /// Creates a new instance of <see cref="DomainException"/> with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    protected DomainException(string message, Exception innerException) 
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a requested book is not found.
/// </summary>
public class BookNotFoundException : DomainException
{
    /// <summary>
    /// Gets the ID of the book that was not found.
    /// </summary>
    public int BookId { get; }

    /// <summary>
    /// Creates a new instance of <see cref="BookNotFoundException"/>.
    /// </summary>
    /// <param name="bookId">The ID of the book that was not found.</param>
    public BookNotFoundException(int bookId) 
        : base($"Book with ID '{bookId}' was not found.")
    {
        BookId = bookId;
    }
}

/// <summary>
/// Exception thrown when a requested author is not found.
/// </summary>
public class AuthorNotFoundException : DomainException
{
    /// <summary>
    /// Gets the ID of the author that was not found.
    /// </summary>
    public int AuthorId { get; }

    /// <summary>
    /// Creates a new instance of <see cref="AuthorNotFoundException"/>.
    /// </summary>
    /// <param name="authorId">The ID of the author that was not found.</param>
    public AuthorNotFoundException(int authorId) 
        : base($"Author with ID '{authorId}' was not found.")
    {
        AuthorId = authorId;
    }
}

/// <summary>
/// Exception thrown when a requested genre is not found.
/// </summary>
public class GenreNotFoundException : DomainException
{
    /// <summary>
    /// Gets the ID of the genre that was not found.
    /// </summary>
    public int GenreId { get; }

    /// <summary>
    /// Creates a new instance of <see cref="GenreNotFoundException"/>.
    /// </summary>
    /// <param name="genreId">The ID of the genre that was not found.</param>
    public GenreNotFoundException(int genreId) 
        : base($"Genre with ID '{genreId}' was not found.")
    {
        GenreId = genreId;
    }
}

/// <summary>
/// Exception thrown when a requested review is not found.
/// </summary>
public class ReviewNotFoundException : DomainException
{
    /// <summary>
    /// Gets the ID of the review that was not found.
    /// </summary>
    public int ReviewId { get; }

    /// <summary>
    /// Creates a new instance of <see cref="ReviewNotFoundException"/>.
    /// </summary>
    /// <param name="reviewId">The ID of the review that was not found.</param>
    public ReviewNotFoundException(int reviewId) 
        : base($"Review with ID '{reviewId}' was not found.")
    {
        ReviewId = reviewId;
    }
}

/// <summary>
/// Exception thrown when attempting to create a duplicate ISBN.
/// </summary>
public class DuplicateIsbnException : DomainException
{
    /// <summary>
    /// Gets the ISBN that was duplicated.
    /// </summary>
    public string Isbn { get; }

    /// <summary>
    /// Creates a new instance of <see cref="DuplicateIsbnException"/>.
    /// </summary>
    /// <param name="isbn">The duplicated ISBN.</param>
    public DuplicateIsbnException(string isbn) 
        : base($"A book with ISBN '{isbn}' already exists.")
    {
        Isbn = isbn;
    }
}

/// <summary>
/// Exception thrown when an invalid rating is provided.
/// </summary>
public class InvalidRatingException : DomainException
{
    /// <summary>
    /// Gets the invalid rating value.
    /// </summary>
    public int Rating { get; }

    /// <summary>
    /// Creates a new instance of <see cref="InvalidRatingException"/>.
    /// </summary>
    /// <param name="rating">The invalid rating value.</param>
    public InvalidRatingException(int rating) 
        : base($"Rating '{rating}' is invalid. Rating must be between 1 and 5.")
    {
        Rating = rating;
    }
}
