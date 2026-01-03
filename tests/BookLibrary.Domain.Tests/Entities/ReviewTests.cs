using BookLibrary.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace BookLibrary.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Review"/> entity.
/// </summary>
public class ReviewTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidRatingAndName_ShouldCreateReview()
    {
        // Arrange
        const int rating = 5;
        const string reviewerName = "John Doe";

        // Act
        var review = new Review(rating, reviewerName);

        // Assert
        review.Rating.Should().Be(rating);
        review.ReviewerName.Should().Be(reviewerName);
        review.Comment.Should().BeNull();
        review.BookId.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithFullDetails_ShouldCreateReviewWithAllProperties()
    {
        // Arrange
        const int rating = 4;
        const string reviewerName = "Jane Smith";
        const string comment = "Great book!";
        const int bookId = 1;

        // Act
        var review = new Review(rating, reviewerName, comment, bookId);

        // Assert
        review.Rating.Should().Be(rating);
        review.ReviewerName.Should().Be(reviewerName);
        review.Comment.Should().Be(comment);
        review.BookId.Should().Be(bookId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Constructor_WithInvalidRating_ShouldThrowArgumentOutOfRangeException(int invalidRating)
    {
        // Act
        var act = () => new Review(invalidRating, "Reviewer");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("rating")
            .WithMessage("*between 1 and 5*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidReviewerName_ShouldThrowArgumentException(string? invalidName)
    {
        // Act
        var act = () => new Review(5, invalidName!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("reviewerName")
            .WithMessage("*cannot be empty*");
    }

    #endregion

    #region Rating Boundary Tests

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Constructor_WithValidRating_ShouldAcceptAllValidValues(int validRating)
    {
        // Act
        var review = new Review(validRating, "Reviewer");

        // Assert
        review.Rating.Should().Be(validRating);
    }

    [Fact]
    public void MinRating_ShouldBe1()
    {
        Review.MinRating.Should().Be(1);
    }

    [Fact]
    public void MaxRating_ShouldBe5()
    {
        Review.MaxRating.Should().Be(5);
    }

    #endregion

    #region Default Constructor Tests

    [Fact]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var review = new Review();

        // Assert
        review.Id.Should().Be(0);
        review.Rating.Should().Be(0);
        review.Comment.Should().BeNull();
        review.ReviewerName.Should().BeEmpty();
        review.ReviewerEmail.Should().BeNull();
        review.BookId.Should().Be(0);
        review.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void DefaultConstructor_ShouldSetCreatedAtToRecentTime()
    {
        // Act
        var review = new Review();

        // Assert
        review.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region UpdateRating Tests

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void UpdateRating_WithValidRating_ShouldUpdateRating(int newRating)
    {
        // Arrange
        var review = new Review(3, "Reviewer");

        // Act
        review.UpdateRating(newRating);

        // Assert
        review.Rating.Should().Be(newRating);
    }

    [Fact]
    public void UpdateRating_ShouldSetUpdatedAt()
    {
        // Arrange
        var review = new Review(3, "Reviewer");
        review.UpdatedAt.Should().BeNull();

        // Act
        review.UpdateRating(5);

        // Assert
        review.UpdatedAt.Should().NotBeNull();
        review.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    public void UpdateRating_WithInvalidRating_ShouldThrowArgumentOutOfRangeException(int invalidRating)
    {
        // Arrange
        var review = new Review(3, "Reviewer");

        // Act
        var act = () => review.UpdateRating(invalidRating);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("rating");
    }

    #endregion

    #region UpdateComment Tests

    [Fact]
    public void UpdateComment_WithNewComment_ShouldUpdateComment()
    {
        // Arrange
        var review = new Review(5, "Reviewer", "Original comment", 1);
        const string newComment = "Updated comment";

        // Act
        review.UpdateComment(newComment);

        // Assert
        review.Comment.Should().Be(newComment);
    }

    [Fact]
    public void UpdateComment_WithNull_ShouldSetCommentToNull()
    {
        // Arrange
        var review = new Review(5, "Reviewer", "Original comment", 1);

        // Act
        review.UpdateComment(null);

        // Assert
        review.Comment.Should().BeNull();
    }

    [Fact]
    public void UpdateComment_ShouldSetUpdatedAt()
    {
        // Arrange
        var review = new Review(5, "Reviewer");
        review.UpdatedAt.Should().BeNull();

        // Act
        review.UpdateComment("New comment");

        // Assert
        review.UpdatedAt.Should().NotBeNull();
        review.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var review = new Review();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var updatedAt = DateTime.UtcNow;

        // Act
        review.Id = 1;
        review.Rating = 4;
        review.Comment = "Great!";
        review.ReviewerName = "Test Reviewer";
        review.ReviewerEmail = "test@example.com";
        review.BookId = 5;
        review.CreatedAt = createdAt;
        review.UpdatedAt = updatedAt;

        // Assert
        review.Id.Should().Be(1);
        review.Rating.Should().Be(4);
        review.Comment.Should().Be("Great!");
        review.ReviewerName.Should().Be("Test Reviewer");
        review.ReviewerEmail.Should().Be("test@example.com");
        review.BookId.Should().Be(5);
        review.CreatedAt.Should().Be(createdAt);
        review.UpdatedAt.Should().Be(updatedAt);
    }

    #endregion
}
