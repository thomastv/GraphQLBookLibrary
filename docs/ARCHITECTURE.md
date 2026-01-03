# Architecture Guide

This document describes the architecture and design decisions of the GraphQL Book Library API.

## Overview

The application follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────────┐
│                         Clients                                  │
│              (Nitro IDE, Apollo Client, etc.)                   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      GraphQL Layer                               │
│                     (Hot Chocolate)                              │
│  ┌──────────┐  ┌──────────┐  ┌─────────────┐  ┌────────────┐   │
│  │ Queries  │  │Mutations │  │Subscriptions│  │ DataLoaders│   │
│  └──────────┘  └──────────┘  └─────────────┘  └────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Data Access Layer                           │
│                   (Entity Framework Core)                        │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                    AppDbContext                           │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                       Domain Layer                               │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐        │
│  │  Author  │  │   Book   │  │  Review  │  │  Genre   │        │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘        │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                        Database                                  │
│                        (SQLite)                                  │
└─────────────────────────────────────────────────────────────────┘
```

## Projects

### BookLibrary.Domain

**Purpose**: Contains domain entities and business logic.

**Dependencies**: None (pure .NET)

**Contents**:
- `Entities/` - Domain models (Author, Book, Genre, Review)
- `Exceptions/` - Custom domain exceptions

**Design Principles**:
- Rich domain models with validation
- No dependencies on infrastructure
- Business rules enforced in entities

### BookLibrary.Api

**Purpose**: GraphQL API and data access.

**Dependencies**: 
- BookLibrary.Domain
- Hot Chocolate (GraphQL)
- Entity Framework Core (ORM)
- SQLite (Database)

**Contents**:
- `Data/` - DbContext and seed data
- `GraphQL/Queries/` - Query resolvers
- `GraphQL/Mutations/` - Mutation resolvers
- `GraphQL/Subscriptions/` - Real-time subscriptions
- `GraphQL/DataLoaders/` - Batch loading
- `GraphQL/Types/` - Input types

## Hot Chocolate Patterns

### Code-First Schema

The GraphQL schema is generated from C# classes using attributes:

```csharp
[QueryType]
public static class BookQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Book> GetBooks(AppDbContext context)
        => context.Books;
}
```

### Type Registration

Types are automatically discovered using:

```csharp
builder.Services
    .AddGraphQLServer()
    .AddApiTypes()  // Discovers all [QueryType], [MutationType], etc.
```

### DataLoaders

DataLoaders prevent N+1 query problems by batching requests:

```csharp
[DataLoader]
public static async Task<Dictionary<int, Author>> GetAuthorsByIdAsync(
    IReadOnlyList<int> ids,
    AppDbContext context,
    CancellationToken ct)
{
    return await context.Authors
        .Where(a => ids.Contains(a.Id))
        .ToDictionaryAsync(a => a.Id, ct);
}
```

### Middleware Pipeline

Middleware is applied in order:

```csharp
[UsePaging]       // 1. Pagination
[UseProjection]   // 2. Field selection
[UseFiltering]    // 3. Where clauses
[UseSorting]      // 4. Order by
```

## Entity Framework Patterns

### DbContext

Single context managing all entities:

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Review> Reviews => Set<Review>();
}
```

### Relationships

Configured in `OnModelCreating`:

```csharp
// One-to-Many: Author -> Books
modelBuilder.Entity<Book>()
    .HasOne(b => b.Author)
    .WithMany(a => a.Books)
    .HasForeignKey(b => b.AuthorId);

// Many-to-Many: Books <-> Genres
modelBuilder.Entity<Book>()
    .HasMany(b => b.Genres)
    .WithMany(g => g.Books);
```

## Domain Model

### Author

```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Biography { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public ICollection<Book> Books { get; set; }
    
    // Computed property
    public int BookCount => Books.Count;
}
```

### Book

```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Isbn { get; set; }
    public string? Description { get; set; }
    public DateTime? PublishedDate { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    public ICollection<Genre> Genres { get; set; }
    public ICollection<Review> Reviews { get; set; }
    
    // Computed properties
    public double? AverageRating => Reviews.Any() 
        ? Reviews.Average(r => r.Rating) 
        : null;
    public int ReviewCount => Reviews.Count;
}
```

### Review

```csharp
public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }  // 1-5
    public string? Comment { get; set; }
    public string ReviewerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
}
```

### Genre

```csharp
public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Book> Books { get; set; }
}
```

## GraphQL Schema

Generated schema structure:

```graphql
type Query {
  books(where: BookFilterInput, order: [BookSortInput!]): [Book!]!
  bookById(id: Int!): Book
  authors(where: AuthorFilterInput, order: [AuthorSortInput!]): [Author!]!
  authorById(id: Int!): Author
  genres: [Genre!]!
  genreById(id: Int!): Genre
  reviews(where: ReviewFilterInput, order: [ReviewSortInput!]): [Review!]!
  reviewById(id: Int!): Review
}

type Mutation {
  addBook(input: AddBookInput!): AddBookPayload!
  updateBook(input: UpdateBookInput!): UpdateBookPayload!
  deleteBook(id: Int!): DeleteBookPayload!
  addAuthor(input: AddAuthorInput!): AddAuthorPayload!
  updateAuthor(input: UpdateAuthorInput!): UpdateAuthorPayload!
  deleteAuthor(id: Int!): DeleteAuthorPayload!
  addReview(input: AddReviewInput!): AddReviewPayload!
  updateReview(input: UpdateReviewInput!): UpdateReviewPayload!
  deleteReview(id: Int!): DeleteReviewPayload!
  addGenre(input: AddGenreInput!): AddGenrePayload!
}

type Subscription {
  onBookAdded: Book!
  onReviewAdded: Review!
}
```

## Error Handling

### Domain Exceptions

Custom exceptions for domain errors:

```csharp
public class BookNotFoundException : Exception
{
    public BookNotFoundException(int id) 
        : base($"Book with ID {id} was not found.") { }
}
```

### GraphQL Errors

Errors are returned in the payload:

```graphql
mutation {
  addBook(input: { title: "Test", authorId: 999 }) {
    book { id }
    errors {
      __typename
      ... on AuthorNotFoundException {
        message
      }
    }
  }
}
```

## Configuration

### Program.cs Structure

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. GraphQL
builder.Services
    .AddGraphQLServer()
    .AddApiTypes()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddMutationConventions()
    .AddInMemorySubscriptions()
    .RegisterDbContextFactory<AppDbContext>();

var app = builder.Build();

// 3. Database initialization
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    await SeedData.InitializeAsync(context);
}

// 4. Middleware
app.MapGraphQL();

app.Run();
```

## Testing Strategy

### Unit Tests (Domain)

Test domain logic in isolation:

```csharp
[Fact]
public void Book_WithReviews_ShouldCalculateAverageRating()
{
    var book = new Book("Test");
    book.Reviews.Add(new Review(4, "Reviewer1"));
    book.Reviews.Add(new Review(5, "Reviewer2"));
    
    book.AverageRating.Should().Be(4.5);
}
```

### Integration Tests (Future)

Test GraphQL queries against in-memory database:

```csharp
[Fact]
public async Task GetBooks_ShouldReturnAllBooks()
{
    var result = await executor.ExecuteAsync("query { books { id title } }");
    result.Errors.Should().BeNullOrEmpty();
}
```

## Performance Considerations

### DataLoaders

Prevent N+1 by batching:

```
Without DataLoader:
Query 1: SELECT * FROM Books
Query 2: SELECT * FROM Authors WHERE Id = 1
Query 3: SELECT * FROM Authors WHERE Id = 2
... (N queries for N books)

With DataLoader:
Query 1: SELECT * FROM Books
Query 2: SELECT * FROM Authors WHERE Id IN (1, 2, 3...)
```

### Projections

Only select needed columns:

```csharp
[UseProjection]
public static IQueryable<Book> GetBooks(AppDbContext context)
    => context.Books;

// Query: { books { title } }
// SQL: SELECT Title FROM Books
```

### Filtering at Database Level

Filters are converted to SQL:

```csharp
[UseFiltering]
public static IQueryable<Book> GetBooks(AppDbContext context)
    => context.Books;

// Query: { books(where: { title: { contains: "1984" } }) { title } }
// SQL: SELECT * FROM Books WHERE Title LIKE '%1984%'
```

## Security Considerations

### Input Validation

Validation in domain entities:

```csharp
public Author(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Name cannot be empty");
    Name = name;
}
```

### Query Depth Limiting

Configure max query depth:

```csharp
builder.Services
    .AddGraphQLServer()
    .SetMaxAllowedExecutionDepth(15);
```

### Introspection

Disable in production if needed:

```csharp
builder.Services
    .AddGraphQLServer()
    .ModifyOptions(o => o.EnableIntrospection = env.IsDevelopment());
```
