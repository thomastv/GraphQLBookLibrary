using BookLibrary.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace BookLibrary.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Genre"/> entity.
/// </summary>
public class GenreTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidName_ShouldCreateGenre()
    {
        // Arrange
        const string name = "Science Fiction";

        // Act
        var genre = new Genre(name);

        // Assert
        genre.Name.Should().Be(name);
        genre.Description.Should().BeNull();
        genre.Books.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithNameAndDescription_ShouldCreateGenreWithDescription()
    {
        // Arrange
        const string name = "Fantasy";
        const string description = "Stories featuring magical elements";

        // Act
        var genre = new Genre(name, description);

        // Assert
        genre.Name.Should().Be(name);
        genre.Description.Should().Be(description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Act
        var act = () => new Genre(invalidName!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("name")
            .WithMessage("*cannot be empty*");
    }

    #endregion

    #region Default Constructor Tests

    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var genre = new Genre();

        // Assert
        genre.Id.Should().Be(0);
        genre.Name.Should().BeEmpty();
        genre.Description.Should().BeNull();
        genre.Books.Should().BeEmpty();
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var genre = new Genre();

        // Act
        genre.Id = 1;
        genre.Name = "Horror";
        genre.Description = "Scary stories";

        // Assert
        genre.Id.Should().Be(1);
        genre.Name.Should().Be("Horror");
        genre.Description.Should().Be("Scary stories");
    }

    [Fact]
    public void Books_ShouldAllowAddingBooks()
    {
        // Arrange
        var genre = new Genre("Mystery");
        var book1 = new Book("Murder on the Orient Express");
        var book2 = new Book("And Then There Were None");

        // Act
        genre.Books.Add(book1);
        genre.Books.Add(book2);

        // Assert
        genre.Books.Should().HaveCount(2);
        genre.Books.Should().Contain(book1);
        genre.Books.Should().Contain(book2);
    }

    #endregion
}
