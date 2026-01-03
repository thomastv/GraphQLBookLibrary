# BookLibrary.Domain

The domain layer contains the core business entities, value objects, and domain-specific exceptions for the Book Library application.

## 📁 Structure

```
BookLibrary.Domain/
├── Entities/
│   ├── Author.cs      # Author entity with books relationship
│   ├── Book.cs        # Book entity with author, genres, reviews
│   ├── Genre.cs       # Genre entity for categorization
│   └── Review.cs      # Review entity with rating validation
└── Exceptions/
    └── DomainExceptions.cs  # Domain-specific exceptions
```

## 📊 Entity Relationships

```
┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│   Author    │ 1   N │    Book     │ N   M │   Genre     │
│             │───────│             │───────│             │
│ - Id        │       │ - Id        │       │ - Id        │
│ - Name      │       │ - Title     │       │ - Name      │
│ - Biography │       │ - ISBN      │       │ - Description│
│ - DateOfBirth       │ - Description        └─────────────┘
│ - Nationality│      │ - PublishedDate│
└─────────────┘       │ - AuthorId  │
                      │ - PageCount │
                      └──────┬──────┘
                             │ 1
                             │
                             │ N
                      ┌──────┴──────┐
                      │   Review    │
                      │             │
                      │ - Id        │
                      │ - Rating    │
                      │ - Comment   │
                      │ - ReviewerName│
                      │ - BookId    │
                      └─────────────┘
```

## 🏷️ Entities

### Author

Represents a book author with biographical information.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Unique identifier |
| `Name` | `string` | Full name (required) |
| `Biography` | `string?` | Brief biography |
| `DateOfBirth` | `DateTime?` | Birth date |
| `Nationality` | `string?` | Country of origin |
| `ImageUrl` | `string?` | Profile image URL |
| `Books` | `ICollection<Book>` | Books by this author |
| `BookCount` | `int` | Computed: number of books |

### Book

Represents a book in the library.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Unique identifier |
| `Title` | `string` | Book title (required) |
| `Isbn` | `string?` | ISBN number |
| `Description` | `string?` | Summary/description |
| `PublishedDate` | `DateTime?` | Publication date |
| `PageCount` | `int?` | Number of pages |
| `CoverImageUrl` | `string?` | Cover image URL |
| `Language` | `string` | Language (default: English) |
| `Publisher` | `string?` | Publisher name |
| `AuthorId` | `int` | Foreign key to Author |
| `Author` | `Author` | Navigation property |
| `Genres` | `ICollection<Genre>` | Associated genres |
| `Reviews` | `ICollection<Review>` | Book reviews |
| `AverageRating` | `double?` | Computed: average rating |
| `ReviewCount` | `int` | Computed: number of reviews |

### Genre

Represents a book category/genre.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Unique identifier |
| `Name` | `string` | Genre name (required) |
| `Description` | `string?` | Genre description |
| `Books` | `ICollection<Book>` | Books in this genre |

### Review

Represents a user review for a book.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Unique identifier |
| `Rating` | `int` | Rating 1-5 (validated) |
| `Comment` | `string?` | Review text |
| `ReviewerName` | `string` | Reviewer's name (required) |
| `ReviewerEmail` | `string?` | Reviewer's email |
| `CreatedAt` | `DateTime` | Creation timestamp |
| `UpdatedAt` | `DateTime?` | Last update timestamp |
| `BookId` | `int` | Foreign key to Book |
| `Book` | `Book` | Navigation property |

## ⚠️ Domain Exceptions

All exceptions inherit from `DomainException`:

| Exception | Description | Properties |
|-----------|-------------|------------|
| `BookNotFoundException` | Book not found | `BookId` |
| `AuthorNotFoundException` | Author not found | `AuthorId` |
| `GenreNotFoundException` | Genre not found | `GenreId` |
| `ReviewNotFoundException` | Review not found | `ReviewId` |
| `DuplicateIsbnException` | ISBN already exists | `Isbn` |
| `InvalidRatingException` | Rating not 1-5 | `Rating` |

## 🔒 Business Rules

### Author
- Name is required and cannot be empty

### Book
- Title is required and cannot be empty
- AuthorId must be valid (> 0)
- ISBN should be unique (enforced at data layer)

### Review
- Rating must be between 1 and 5 (inclusive)
- ReviewerName is required
- CreatedAt is automatically set to UTC now
- UpdatedAt is set when rating or comment changes

## 💡 Usage Examples

### Creating an Author

```csharp
var author = new Author("Douglas Adams")
{
    Biography = "English author and scriptwriter",
    DateOfBirth = new DateTime(1952, 3, 11),
    Nationality = "British"
};
```

### Creating a Book

```csharp
var book = new Book("The Hitchhiker's Guide to the Galaxy")
{
    Isbn = "978-0345391803",
    Description = "A comedy science fiction series",
    AuthorId = 1,
    PublishedDate = new DateTime(1979, 10, 12),
    PageCount = 224
};
```

### Creating a Review

```csharp
var review = new Review(5, "John Doe")
{
    Comment = "Absolutely hilarious! A must-read.",
    BookId = 1
};
```

### Handling Domain Exceptions

```csharp
try
{
    var review = new Review(6, "Jane Doe"); // Invalid rating
}
catch (ArgumentOutOfRangeException ex)
{
    // Handle invalid rating
}
```
