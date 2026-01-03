namespace BookLibrary.Api.GraphQL.Types;

/// <summary>
/// Input type for adding a new book.
/// </summary>
/// <param name="Title">The book title.</param>
/// <param name="AuthorId">The author's ID.</param>
/// <param name="Isbn">Optional ISBN.</param>
/// <param name="Description">Optional description.</param>
/// <param name="PublishedDate">Optional publication date.</param>
/// <param name="PageCount">Optional page count.</param>
/// <param name="Publisher">Optional publisher name.</param>
/// <param name="Language">Optional language (defaults to English).</param>
/// <param name="GenreIds">Optional list of genre IDs.</param>
public record AddBookInput(
    string Title,
    int AuthorId,
    string? Isbn = null,
    string? Description = null,
    DateTime? PublishedDate = null,
    int? PageCount = null,
    string? Publisher = null,
    string? Language = null,
    List<int>? GenreIds = null);

/// <summary>
/// Input type for updating a book.
/// </summary>
/// <param name="Id">The book ID to update.</param>
/// <param name="Title">Optional new title.</param>
/// <param name="Isbn">Optional new ISBN.</param>
/// <param name="Description">Optional new description.</param>
/// <param name="PublishedDate">Optional new publication date.</param>
/// <param name="PageCount">Optional new page count.</param>
/// <param name="Publisher">Optional new publisher.</param>
/// <param name="Language">Optional new language.</param>
/// <param name="GenreIds">Optional new list of genre IDs.</param>
public record UpdateBookInput(
    int Id,
    string? Title = null,
    string? Isbn = null,
    string? Description = null,
    DateTime? PublishedDate = null,
    int? PageCount = null,
    string? Publisher = null,
    string? Language = null,
    List<int>? GenreIds = null);

/// <summary>
/// Input type for adding a new author.
/// </summary>
/// <param name="Name">The author's name.</param>
/// <param name="Biography">Optional biography.</param>
/// <param name="DateOfBirth">Optional date of birth.</param>
/// <param name="Nationality">Optional nationality.</param>
/// <param name="ImageUrl">Optional image URL.</param>
public record AddAuthorInput(
    string Name,
    string? Biography = null,
    DateTime? DateOfBirth = null,
    string? Nationality = null,
    string? ImageUrl = null);

/// <summary>
/// Input type for updating an author.
/// </summary>
/// <param name="Id">The author ID to update.</param>
/// <param name="Name">Optional new name.</param>
/// <param name="Biography">Optional new biography.</param>
/// <param name="DateOfBirth">Optional new date of birth.</param>
/// <param name="Nationality">Optional new nationality.</param>
/// <param name="ImageUrl">Optional new image URL.</param>
public record UpdateAuthorInput(
    int Id,
    string? Name = null,
    string? Biography = null,
    DateTime? DateOfBirth = null,
    string? Nationality = null,
    string? ImageUrl = null);

/// <summary>
/// Input type for adding a new review.
/// </summary>
/// <param name="BookId">The book ID to review.</param>
/// <param name="Rating">The rating (1-5).</param>
/// <param name="ReviewerName">The reviewer's name.</param>
/// <param name="Comment">Optional review comment.</param>
/// <param name="ReviewerEmail">Optional reviewer email.</param>
public record AddReviewInput(
    int BookId,
    int Rating,
    string ReviewerName,
    string? Comment = null,
    string? ReviewerEmail = null);

/// <summary>
/// Input type for updating a review.
/// </summary>
/// <param name="Id">The review ID to update.</param>
/// <param name="Rating">Optional new rating.</param>
/// <param name="Comment">Optional new comment.</param>
public record UpdateReviewInput(
    int Id,
    int? Rating = null,
    string? Comment = null);

/// <summary>
/// Input type for adding a new genre.
/// </summary>
/// <param name="Name">The genre name.</param>
/// <param name="Description">Optional description.</param>
public record AddGenreInput(
    string Name,
    string? Description = null);
