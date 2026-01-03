namespace BookLibrary.Domain.Entities;

/// <summary>
/// Represents a review for a book.
/// </summary>
public class Review
{
    /// <summary>
    /// The minimum allowed rating value.
    /// </summary>
    public const int MinRating = 1;

    /// <summary>
    /// The maximum allowed rating value.
    /// </summary>
    public const int MaxRating = 5;

    /// <summary>
    /// Gets or sets the unique identifier for the review.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the rating (1-5 stars).
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets the review comment/text.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets the name of the reviewer.
    /// </summary>
    public string ReviewerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reviewer's email (optional).
    /// </summary>
    public string? ReviewerEmail { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the review was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the review was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the book being reviewed.
    /// </summary>
    public int BookId { get; set; }

    /// <summary>
    /// Gets or sets the book being reviewed.
    /// </summary>
    public Book Book { get; set; } = null!;

    /// <summary>
    /// Creates a new instance of <see cref="Review"/>.
    /// </summary>
    public Review() { }

    /// <summary>
    /// Creates a new instance of <see cref="Review"/> with rating and reviewer name.
    /// </summary>
    /// <param name="rating">The rating (1-5).</param>
    /// <param name="reviewerName">The name of the reviewer.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when rating is not between 1 and 5.</exception>
    /// <exception cref="ArgumentException">Thrown when reviewer name is null or empty.</exception>
    public Review(int rating, string reviewerName)
    {
        ValidateRating(rating);
        
        if (string.IsNullOrWhiteSpace(reviewerName))
            throw new ArgumentException("Reviewer name cannot be empty.", nameof(reviewerName));
        
        Rating = rating;
        ReviewerName = reviewerName;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Review"/> with full details.
    /// </summary>
    /// <param name="rating">The rating (1-5).</param>
    /// <param name="reviewerName">The name of the reviewer.</param>
    /// <param name="comment">The review comment.</param>
    /// <param name="bookId">The book's ID.</param>
    public Review(int rating, string reviewerName, string? comment, int bookId)
        : this(rating, reviewerName)
    {
        Comment = comment;
        BookId = bookId;
    }

    /// <summary>
    /// Updates the rating with validation.
    /// </summary>
    /// <param name="newRating">The new rating value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when rating is not between 1 and 5.</exception>
    public void UpdateRating(int newRating)
    {
        ValidateRating(newRating);
        Rating = newRating;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the comment.
    /// </summary>
    /// <param name="newComment">The new comment.</param>
    public void UpdateComment(string? newComment)
    {
        Comment = newComment;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates that the rating is within the allowed range.
    /// </summary>
    /// <param name="rating">The rating to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when rating is not between 1 and 5.</exception>
    private static void ValidateRating(int rating)
    {
        if (rating < MinRating || rating > MaxRating)
            throw new ArgumentOutOfRangeException(
                nameof(rating), 
                $"Rating must be between {MinRating} and {MaxRating}.");
    }
}
