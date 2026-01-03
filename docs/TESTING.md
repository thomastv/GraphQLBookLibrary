# Testing Guide

This guide covers testing practices and conventions for the GraphQL Book Library project.

## Test Projects

| Project | Description | Framework |
|---------|-------------|-----------|
| `BookLibrary.Domain.Tests` | Domain entity and exception tests | xUnit + FluentAssertions |

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run with Verbose Output

```bash
dotnet test --verbosity normal
```

### Run Specific Test Project

```bash
dotnet test tests/BookLibrary.Domain.Tests
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~AuthorTests"
```

### Run Specific Test Method

```bash
dotnet test --filter "FullyQualifiedName~Author_WithValidName_ShouldSetName"
```

### Run Tests with Code Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Coverage reports are generated in `TestResults/` folder.

### Generate HTML Coverage Report

First, install the report generator:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Then generate the report:

```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Open `coveragereport/index.html` to view the report.

## Test Organization

### Naming Convention

Tests follow the pattern: `MethodName_Scenario_ExpectedResult`

```csharp
[Fact]
public void Constructor_WithValidName_ShouldSetName()

[Fact]
public void Constructor_WithEmptyName_ShouldThrowArgumentException()

[Theory]
[InlineData(0)]
[InlineData(6)]
public void SetRating_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int rating)
```

### Test Structure (Arrange-Act-Assert)

```csharp
[Fact]
public void AddBook_WithValidBook_ShouldAddToCollection()
{
    // Arrange
    var author = new Author("Test Author");
    var book = new Book("Test Book") { AuthorId = 1 };

    // Act
    author.Books.Add(book);

    // Assert
    author.Books.Should().Contain(book);
    author.BookCount.Should().Be(1);
}
```

## Domain Tests

### Entity Tests

Located in `tests/BookLibrary.Domain.Tests/Entities/`

#### AuthorTests.cs

Tests for the `Author` entity:
- Constructor validation
- Name property validation
- Book collection management
- BookCount calculation

#### BookTests.cs

Tests for the `Book` entity:
- Constructor validation
- Title and ISBN validation
- Author relationship
- Genre collection management
- Review collection management
- AverageRating calculation

#### ReviewTests.cs

Tests for the `Review` entity:
- Constructor validation
- Rating validation (1-5 range)
- Reviewer name validation
- UpdateRating method
- Timestamp handling

#### GenreTests.cs

Tests for the `Genre` entity:
- Constructor validation
- Name validation
- Book collection management

### Exception Tests

Located in `tests/BookLibrary.Domain.Tests/Exceptions/`

#### DomainExceptionTests.cs

Tests for custom domain exceptions:
- `BookNotFoundException`
- `AuthorNotFoundException`
- `ReviewNotFoundException`
- `GenreNotFoundException`
- `DuplicateIsbnException`
- `InvalidRatingException`

## Writing Tests

### Using xUnit

```csharp
using Xunit;

public class MyTests
{
    [Fact]
    public void SimpleTest()
    {
        // Single test case
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ParameterizedTest(int value)
    {
        // Runs for each InlineData
    }
}
```

### Using FluentAssertions

```csharp
using FluentAssertions;

// Basic assertions
result.Should().Be(expected);
result.Should().NotBeNull();
result.Should().BeTrue();

// String assertions
name.Should().StartWith("Test");
name.Should().Contain("Author");
name.Should().BeEmpty();

// Collection assertions
books.Should().HaveCount(5);
books.Should().Contain(book);
books.Should().BeEmpty();
books.Should().OnlyContain(b => b.IsPublished);

// Exception assertions
action.Should().Throw<ArgumentException>();
action.Should().Throw<ArgumentException>()
    .WithMessage("*cannot be empty*");

// Numeric assertions
rating.Should().BeInRange(1, 5);
count.Should().BeGreaterThan(0);
average.Should().BeApproximately(4.5, 0.01);
```

### Test Lifecycle

```csharp
public class MyTests : IAsyncLifetime
{
    private MyService _service;

    public async Task InitializeAsync()
    {
        // Called before each test
        _service = await CreateServiceAsync();
    }

    public async Task DisposeAsync()
    {
        // Called after each test
        await _service.DisposeAsync();
    }

    [Fact]
    public async Task MyTest()
    {
        // Test using _service
    }
}
```

### Test Fixtures (Shared Context)

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    public AppDbContext DbContext { get; private set; }

    public async Task InitializeAsync()
    {
        // Setup shared resources
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"Test_{Guid.NewGuid()}")
            .Options;
        DbContext = new AppDbContext(options);
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }
}

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }

[Collection("Database")]
public class MyDatabaseTests
{
    private readonly DatabaseFixture _fixture;

    public MyDatabaseTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
}
```

## Mocking

For unit tests that need mocking, use Moq (can be added to project):

```bash
dotnet add tests/BookLibrary.Domain.Tests package Moq
```

```csharp
using Moq;

var mockRepository = new Mock<IBookRepository>();
mockRepository
    .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(new Book("Test"));

var service = new BookService(mockRepository.Object);
```

## Best Practices

### 1. One Assertion Per Test (Preferred)

```csharp
// Good - focused test
[Fact]
public void BookCount_WhenEmpty_ShouldReturnZero()
{
    var author = new Author("Test");
    author.BookCount.Should().Be(0);
}
```

### 2. Test Edge Cases

```csharp
[Theory]
[InlineData("")]
[InlineData(" ")]
[InlineData(null)]
public void Constructor_WithInvalidName_ShouldThrow(string name)
{
    var action = () => new Author(name);
    action.Should().Throw<ArgumentException>();
}
```

### 3. Use Descriptive Names

```csharp
// Bad
[Fact]
public void Test1() { }

// Good
[Fact]
public void AverageRating_WithNoReviews_ShouldReturnNull() { }
```

### 4. Keep Tests Independent

Each test should be able to run in isolation without depending on other tests.

### 5. Don't Test Framework Code

Focus on testing your business logic, not EF Core or Hot Chocolate internals.

## Continuous Integration

Add this to your CI pipeline:

```yaml
# GitHub Actions example
- name: Run tests
  run: dotnet test --configuration Release --verbosity normal

- name: Run tests with coverage
  run: dotnet test --collect:"XPlat Code Coverage"

- name: Upload coverage
  uses: codecov/codecov-action@v3
```

## Test Results

After running tests, results are displayed in the console:

```
Test summary: total: 77, failed: 0, succeeded: 77, skipped: 0, duration: 7.7s
```

For detailed results, use `--logger` option:

```bash
dotnet test --logger "trx;LogFileName=test-results.trx"
```
