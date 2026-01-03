using BookLibrary.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace BookLibrary.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Author"/> entity.
/// </summary>
public class AuthorTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidName_ShouldCreateAuthor()
    {
        // Arrange
        const string name = "Douglas Adams";

        // Act
        var author = new Author(name);

        // Assert
        author.Name.Should().Be(name);
        author.Biography.Should().BeNull();
        author.DateOfBirth.Should().BeNull();
        author.Nationality.Should().BeNull();
        author.Books.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithFullDetails_ShouldCreateAuthorWithAllProperties()
    {
        // Arrange
        const string name = "George Orwell";
        const string biography = "English novelist and essayist";
        var dateOfBirth = new DateTime(1903, 6, 25);
        const string nationality = "British";

        // Act
        var author = new Author(name, biography, dateOfBirth, nationality);

        // Assert
        author.Name.Should().Be(name);
        author.Biography.Should().Be(biography);
        author.DateOfBirth.Should().Be(dateOfBirth);
        author.Nationality.Should().Be(nationality);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Act
        var act = () => new Author(invalidName!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("name")
            .WithMessage("*cannot be empty*");
    }

    #endregion

    #region Property Tests

    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var author = new Author();

        // Assert
        author.Id.Should().Be(0);
        author.Name.Should().BeEmpty();
        author.Biography.Should().BeNull();
        author.DateOfBirth.Should().BeNull();
        author.Nationality.Should().BeNull();
        author.ImageUrl.Should().BeNull();
        author.Books.Should().BeEmpty();
    }

    [Fact]
    public void BookCount_WithNoBooks_ShouldReturnZero()
    {
        // Arrange
        var author = new Author("Test Author");

        // Act & Assert
        author.BookCount.Should().Be(0);
    }

    [Fact]
    public void BookCount_WithBooks_ShouldReturnCorrectCount()
    {
        // Arrange
        var author = new Author("Test Author");
        author.Books.Add(new Book("Book 1"));
        author.Books.Add(new Book("Book 2"));
        author.Books.Add(new Book("Book 3"));

        // Act & Assert
        author.BookCount.Should().Be(3);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var author = new Author();
        var dateOfBirth = new DateTime(1965, 7, 31);

        // Act
        author.Id = 1;
        author.Name = "J.K. Rowling";
        author.Biography = "British author";
        author.DateOfBirth = dateOfBirth;
        author.Nationality = "British";
        author.ImageUrl = "https://example.com/image.jpg";

        // Assert
        author.Id.Should().Be(1);
        author.Name.Should().Be("J.K. Rowling");
        author.Biography.Should().Be("British author");
        author.DateOfBirth.Should().Be(dateOfBirth);
        author.Nationality.Should().Be("British");
        author.ImageUrl.Should().Be("https://example.com/image.jpg");
    }

    #endregion
}
