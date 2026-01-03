namespace BookLibrary.Domain.Entities;

/// <summary>
/// Represents an author who writes books.
/// </summary>
public class Author
{
    /// <summary>
    /// Gets or sets the unique identifier for the author.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the author's full name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a brief biography of the author.
    /// </summary>
    public string? Biography { get; set; }

    /// <summary>
    /// Gets or sets the author's date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the author's nationality.
    /// </summary>
    public string? Nationality { get; set; }

    /// <summary>
    /// Gets or sets the URL to the author's profile image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the collection of books written by this author.
    /// </summary>
    public ICollection<Book> Books { get; set; } = new List<Book>();

    /// <summary>
    /// Gets the total number of books written by this author.
    /// </summary>
    public int BookCount => Books.Count;

    /// <summary>
    /// Creates a new instance of <see cref="Author"/>.
    /// </summary>
    public Author() { }

    /// <summary>
    /// Creates a new instance of <see cref="Author"/> with the specified name.
    /// </summary>
    /// <param name="name">The author's full name.</param>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    public Author(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Author name cannot be empty.", nameof(name));
        
        Name = name;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Author"/> with full details.
    /// </summary>
    /// <param name="name">The author's full name.</param>
    /// <param name="biography">A brief biography.</param>
    /// <param name="dateOfBirth">Date of birth.</param>
    /// <param name="nationality">Nationality.</param>
    public Author(string name, string? biography, DateTime? dateOfBirth, string? nationality)
        : this(name)
    {
        Biography = biography;
        DateOfBirth = dateOfBirth;
        Nationality = nationality;
    }
}
