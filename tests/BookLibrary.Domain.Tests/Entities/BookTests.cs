using BookLibrary.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace BookLibrary.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Book"/> entity.
/// </summary>
public class BookTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidTitle_ShouldCreateBook()
    {
        // Arrange
        const string title = "The Hitchhiker's Guide to the Galaxy";

        // Act
        var book = new Book(title);

        // Assert
        book.Title.Should().Be(title);
        book.Isbn.Should().BeNull();
        book.Description.Should().BeNull();
        book.AuthorId.Should().Be(0);
        book.Genres.Should().BeEmpty();
        book.Reviews.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithFullDetails_ShouldCreateBookWithAllProperties()
    {
        // Arrange
        const string title = "1984";
        const string isbn = "978-0451524935";
        const string description = "A dystopian novel";
        const int authorId = 1;

        // Act
        var book = new Book(title, isbn, description, authorId);

        // Assert
        book.Title.Should().Be(title);
        book.Isbn.Should().Be(isbn);
        book.Description.Should().Be(description);
        book.AuthorId.Should().Be(authorId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidTitle_ShouldThrowArgumentException(string? invalidTitle)
    {
        // Act
        var act = () => new Book(invalidTitle!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("title")
            .WithMessage("*cannot be empty*");
    }

    #endregion

    #region Property Tests

    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var book = new Book();

        // Assert
        book.Id.Should().Be(0);
        book.Title.Should().BeEmpty();
        book.Isbn.Should().BeNull();
        book.Description.Should().BeNull();
        book.PublishedDate.Should().BeNull();
        book.PageCount.Should().BeNull();
        book.CoverImageUrl.Should().BeNull();
        book.Language.Should().Be("English");
        book.Publisher.Should().BeNull();
        book.AuthorId.Should().Be(0);
        book.Genres.Should().BeEmpty();
        book.Reviews.Should().BeEmpty();
    }

    [Fact]
    public void Language_ShouldDefaultToEnglish()
    {
        // Arrange & Act
        var book = new Book("Test Book");

        // Assert
        book.Language.Should().Be("English");
    }

    #endregion

    #region AverageRating Tests

    [Fact]
    public void AverageRating_WithNoReviews_ShouldReturnNull()
    {
        // Arrange
        var book = new Book("Test Book");

        // Act & Assert
        book.AverageRating.Should().BeNull();
    }

    [Fact]
    public void AverageRating_WithSingleReview_ShouldReturnThatRating()
    {
        // Arrange
        var book = new Book("Test Book");
        book.Reviews.Add(new Review(5, "Reviewer"));

        // Act & Assert
        book.AverageRating.Should().Be(5.0);
    }

    [Fact]
    public void AverageRating_WithMultipleReviews_ShouldReturnCorrectAverage()
    {
        // Arrange
        var book = new Book("Test Book");
        book.Reviews.Add(new Review(5, "Reviewer 1"));
        book.Reviews.Add(new Review(4, "Reviewer 2"));
        book.Reviews.Add(new Review(3, "Reviewer 3"));

        // Act & Assert
        book.AverageRating.Should().Be(4.0);
    }

    [Fact]
    public void AverageRating_ShouldRoundToTwoDecimalPlaces()
    {
        // Arrange
        var book = new Book("Test Book");
        book.Reviews.Add(new Review(5, "Reviewer 1"));
        book.Reviews.Add(new Review(4, "Reviewer 2"));
        book.Reviews.Add(new Review(4, "Reviewer 3"));

        // Act
        var average = book.AverageRating;

        // Assert (5+4+4)/3 = 4.333...
        average.Should().Be(4.33);
    }

    #endregion

    #region ReviewCount Tests

    [Fact]
    public void ReviewCount_WithNoReviews_ShouldReturnZero()
    {
        // Arrange
        var book = new Book("Test Book");

        // Act & Assert
        book.ReviewCount.Should().Be(0);
    }

    [Fact]
    public void ReviewCount_WithReviews_ShouldReturnCorrectCount()
    {
        // Arrange
        var book = new Book("Test Book");
        book.Reviews.Add(new Review(5, "Reviewer 1"));
        book.Reviews.Add(new Review(4, "Reviewer 2"));

        // Act & Assert
        book.ReviewCount.Should().Be(2);
    }

    #endregion

    #region IsValid Tests

    [Fact]
    public void IsValid_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var book = new Book("Valid Title") { AuthorId = 1 };

        // Act & Assert
        book.IsValid().Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithEmptyTitle_ShouldReturnFalse()
    {
        // Arrange
        var book = new Book { Title = "", AuthorId = 1 };

        // Act & Assert
        book.IsValid().Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithWhitespaceTitle_ShouldReturnFalse()
    {
        // Arrange
        var book = new Book { Title = "   ", AuthorId = 1 };

        // Act & Assert
        book.IsValid().Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithZeroAuthorId_ShouldReturnFalse()
    {
        // Arrange
        var book = new Book("Valid Title") { AuthorId = 0 };

        // Act & Assert
        book.IsValid().Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithNegativeAuthorId_ShouldReturnFalse()
    {
        // Arrange
        var book = new Book("Valid Title") { AuthorId = -1 };

        // Act & Assert
        book.IsValid().Should().BeFalse();
    }

    #endregion

    #region Genre Tests

    [Fact]
    public void Genres_ShouldAllowAddingGenres()
    {
        // Arrange
        var book = new Book("Test Book");
        var genre1 = new Genre("Science Fiction");
        var genre2 = new Genre("Comedy");

        // Act
        book.Genres.Add(genre1);
        book.Genres.Add(genre2);

        // Assert
        book.Genres.Should().HaveCount(2);
        book.Genres.Should().Contain(genre1);
        book.Genres.Should().Contain(genre2);
    }

    #endregion
}
