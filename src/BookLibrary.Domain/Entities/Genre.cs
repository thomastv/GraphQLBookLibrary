namespace BookLibrary.Domain.Entities;

/// <summary>
/// Represents a book genre/category.
/// </summary>
public class Genre
{
    /// <summary>
    /// Gets or sets the unique identifier for the genre.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the genre name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the genre.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of books in this genre.
    /// </summary>
    public ICollection<Book> Books { get; set; } = new List<Book>();

    /// <summary>
    /// Creates a new instance of <see cref="Genre"/>.
    /// </summary>
    public Genre() { }

    /// <summary>
    /// Creates a new instance of <see cref="Genre"/> with the specified name.
    /// </summary>
    /// <param name="name">The genre name.</param>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    public Genre(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Genre name cannot be empty.", nameof(name));
        
        Name = name;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Genre"/> with name and description.
    /// </summary>
    /// <param name="name">The genre name.</param>
    /// <param name="description">A description of the genre.</param>
    public Genre(string name, string? description)
        : this(name)
    {
        Description = description;
    }
}
