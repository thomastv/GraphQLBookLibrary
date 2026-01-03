namespace BookLibrary.Domain.Entities;

/// <summary>
/// Represents a book in the library.
/// </summary>
public class Book
{
    /// <summary>
    /// Gets or sets the unique identifier for the book.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the book title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the book's ISBN (International Standard Book Number).
    /// </summary>
    public string? Isbn { get; set; }

    /// <summary>
    /// Gets or sets a description or summary of the book.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the publication date of the book.
    /// </summary>
    public DateTime? PublishedDate { get; set; }

    /// <summary>
    /// Gets or sets the number of pages in the book.
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// Gets or sets the URL to the book's cover image.
    /// </summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the language of the book.
    /// </summary>
    public string Language { get; set; } = "English";

    /// <summary>
    /// Gets or sets the publisher of the book.
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the author.
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    public Author Author { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of genres associated with this book.
    /// </summary>
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();

    /// <summary>
    /// Gets or sets the collection of reviews for this book.
    /// </summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>
    /// Gets the average rating of the book based on reviews.
    /// Returns null if there are no reviews.
    /// </summary>
    public double? AverageRating => Reviews.Count > 0 
        ? Math.Round(Reviews.Average(r => r.Rating), 2) 
        : null;

    /// <summary>
    /// Gets the total number of reviews for this book.
    /// </summary>
    public int ReviewCount => Reviews.Count;

    /// <summary>
    /// Creates a new instance of <see cref="Book"/>.
    /// </summary>
    public Book() { }

    /// <summary>
    /// Creates a new instance of <see cref="Book"/> with the specified title.
    /// </summary>
    /// <param name="title">The book title.</param>
    /// <exception cref="ArgumentException">Thrown when title is null or empty.</exception>
    public Book(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Book title cannot be empty.", nameof(title));
        
        Title = title;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Book"/> with full details.
    /// </summary>
    /// <param name="title">The book title.</param>
    /// <param name="isbn">The ISBN.</param>
    /// <param name="description">A description of the book.</param>
    /// <param name="authorId">The author's ID.</param>
    public Book(string title, string? isbn, string? description, int authorId)
        : this(title)
    {
        Isbn = isbn;
        Description = description;
        AuthorId = authorId;
    }

    /// <summary>
    /// Validates that the book has all required fields.
    /// </summary>
    /// <returns>True if valid, false otherwise.</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Title) && AuthorId > 0;
    }
}
