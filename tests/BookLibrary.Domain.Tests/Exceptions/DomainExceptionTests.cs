using BookLibrary.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BookLibrary.Domain.Tests.Exceptions;

/// <summary>
/// Unit tests for domain exceptions.
/// </summary>
public class DomainExceptionTests
{
    #region BookNotFoundException Tests

    [Fact]
    public void BookNotFoundException_ShouldContainBookId()
    {
        // Arrange
        const int bookId = 42;

        // Act
        var exception = new BookNotFoundException(bookId);

        // Assert
        exception.BookId.Should().Be(bookId);
        exception.Message.Should().Contain("42");
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public void BookNotFoundException_ShouldInheritFromDomainException()
    {
        // Act
        var exception = new BookNotFoundException(1);

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
        exception.Should().BeAssignableTo<Exception>();
    }

    #endregion

    #region AuthorNotFoundException Tests

    [Fact]
    public void AuthorNotFoundException_ShouldContainAuthorId()
    {
        // Arrange
        const int authorId = 99;

        // Act
        var exception = new AuthorNotFoundException(authorId);

        // Assert
        exception.AuthorId.Should().Be(authorId);
        exception.Message.Should().Contain("99");
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public void AuthorNotFoundException_ShouldInheritFromDomainException()
    {
        // Act
        var exception = new AuthorNotFoundException(1);

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }

    #endregion

    #region GenreNotFoundException Tests

    [Fact]
    public void GenreNotFoundException_ShouldContainGenreId()
    {
        // Arrange
        const int genreId = 7;

        // Act
        var exception = new GenreNotFoundException(genreId);

        // Assert
        exception.GenreId.Should().Be(genreId);
        exception.Message.Should().Contain("7");
        exception.Message.Should().Contain("not found");
    }

    #endregion

    #region ReviewNotFoundException Tests

    [Fact]
    public void ReviewNotFoundException_ShouldContainReviewId()
    {
        // Arrange
        const int reviewId = 123;

        // Act
        var exception = new ReviewNotFoundException(reviewId);

        // Assert
        exception.ReviewId.Should().Be(reviewId);
        exception.Message.Should().Contain("123");
        exception.Message.Should().Contain("not found");
    }

    #endregion

    #region DuplicateIsbnException Tests

    [Fact]
    public void DuplicateIsbnException_ShouldContainIsbn()
    {
        // Arrange
        const string isbn = "978-0451524935";

        // Act
        var exception = new DuplicateIsbnException(isbn);

        // Assert
        exception.Isbn.Should().Be(isbn);
        exception.Message.Should().Contain(isbn);
        exception.Message.Should().Contain("already exists");
    }

    #endregion

    #region InvalidRatingException Tests

    [Fact]
    public void InvalidRatingException_ShouldContainRating()
    {
        // Arrange
        const int rating = 10;

        // Act
        var exception = new InvalidRatingException(rating);

        // Assert
        exception.Rating.Should().Be(rating);
        exception.Message.Should().Contain("10");
        exception.Message.Should().Contain("invalid");
        exception.Message.Should().Contain("between 1 and 5");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(6)]
    [InlineData(100)]
    public void InvalidRatingException_ShouldWorkWithVariousInvalidRatings(int invalidRating)
    {
        // Act
        var exception = new InvalidRatingException(invalidRating);

        // Assert
        exception.Rating.Should().Be(invalidRating);
        exception.Message.Should().Contain(invalidRating.ToString());
    }

    #endregion
}
