# BookLibrary.Domain.Tests

Unit tests for the domain layer of the Book Library application.

## 📁 Structure

```
BookLibrary.Domain.Tests/
├── Entities/
│   ├── AuthorTests.cs      # Author entity tests
│   ├── BookTests.cs        # Book entity tests
│   ├── GenreTests.cs       # Genre entity tests
│   └── ReviewTests.cs      # Review entity tests
└── Exceptions/
    └── DomainExceptionTests.cs  # Domain exception tests
```

## 🧪 Test Categories

### Entity Tests

Tests for domain entity constructors, properties, and business logic:

| Test Class | Coverage |
|------------|----------|
| `AuthorTests` | Constructor validation, property defaults, BookCount computation |
| `BookTests` | Constructor validation, AverageRating calculation, ReviewCount, IsValid |
| `GenreTests` | Constructor validation, property defaults |
| `ReviewTests` | Constructor validation, rating boundaries, UpdateRating, UpdateComment |

### Exception Tests

Tests for domain-specific exceptions:

| Exception | Tests |
|-----------|-------|
| `BookNotFoundException` | Contains BookId, proper message format |
| `AuthorNotFoundException` | Contains AuthorId, proper message format |
| `GenreNotFoundException` | Contains GenreId, proper message format |
| `ReviewNotFoundException` | Contains ReviewId, proper message format |
| `DuplicateIsbnException` | Contains ISBN, proper message format |
| `InvalidRatingException` | Contains Rating, proper message format |

## 🏃 Running Tests

### Run All Domain Tests

```bash
dotnet test tests/BookLibrary.Domain.Tests
```

### Run with Verbose Output

```bash
dotnet test tests/BookLibrary.Domain.Tests --logger "console;verbosity=detailed"
```

### Run Specific Test Class

```bash
dotnet test tests/BookLibrary.Domain.Tests --filter "FullyQualifiedName~BookTests"
```

### Run with Coverage

```bash
dotnet test tests/BookLibrary.Domain.Tests --collect:"XPlat Code Coverage"
```

## 📝 Naming Conventions

Tests follow the pattern: `MethodName_Scenario_ExpectedResult`

Examples:
- `Constructor_WithValidName_ShouldCreateAuthor`
- `AverageRating_WithNoReviews_ShouldReturnNull`
- `UpdateRating_WithInvalidRating_ShouldThrowArgumentOutOfRangeException`

## 🔧 Test Patterns

### Arrange-Act-Assert (AAA)

```csharp
[Fact]
public void Constructor_WithValidName_ShouldCreateAuthor()
{
    // Arrange
    const string name = "Douglas Adams";

    // Act
    var author = new Author(name);

    // Assert
    author.Name.Should().Be(name);
}
```

### Theory for Multiple Test Cases

```csharp
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
```

### Exception Testing

```csharp
[Fact]
public void Constructor_WithInvalidName_ShouldThrowArgumentException()
{
    // Act
    var act = () => new Author("");

    // Assert
    act.Should().Throw<ArgumentException>()
        .WithParameterName("name")
        .WithMessage("*cannot be empty*");
}
```

## 📦 Dependencies

| Package | Purpose |
|---------|---------|
| xUnit | Test framework |
| FluentAssertions | Fluent assertion library |
| Microsoft.NET.Test.Sdk | Test SDK |
| coverlet.collector | Code coverage |
