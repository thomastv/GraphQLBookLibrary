# BookLibrary.Api

The GraphQL API layer for the Book Library application, built with Hot Chocolate.

## 📁 Structure

```
BookLibrary.Api/
├── Data/
│   ├── AppDbContext.cs      # EF Core database context
│   └── SeedData.cs          # Sample data seeder
├── GraphQL/
│   ├── Queries/             # GraphQL query types
│   │   ├── AuthorQueries.cs
│   │   ├── BookQueries.cs
│   │   ├── GenreQueries.cs
│   │   └── ReviewQueries.cs
│   ├── Mutations/           # GraphQL mutation types
│   │   ├── AuthorMutations.cs
│   │   ├── BookMutations.cs
│   │   ├── GenreMutations.cs
│   │   └── ReviewMutations.cs
│   ├── Subscriptions/       # Real-time subscriptions
│   │   └── BookSubscriptions.cs
│   ├── DataLoaders/         # N+1 prevention
│   │   ├── AuthorDataLoader.cs
│   │   └── ReviewDataLoader.cs
│   └── Types/               # Input types
│       └── InputTypes.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

## 🚀 Running the API

```bash
dotnet run
```

Navigate to `https://localhost:5001/graphql` to access the Banana Cake Pop GraphQL IDE.

## 📊 GraphQL Schema

### Queries

| Query | Description | Parameters |
|-------|-------------|------------|
| `books` | Get all books | Filtering, sorting |
| `book` | Get book by ID | `id: Int!` |
| `booksByAuthor` | Get books by author | `authorId: Int!` |
| `topRatedBooks` | Get highest rated books | `count: Int` |
| `searchBooks` | Search books by title/description | `searchTerm: String!` |
| `authors` | Get all authors | Filtering, sorting |
| `author` | Get author by ID | `id: Int!` |
| `authorsByNationality` | Filter authors | `nationality: String!` |
| `searchAuthors` | Search authors by name | `searchTerm: String!` |
| `genres` | Get all genres | Filtering, sorting |
| `genre` | Get genre by ID | `id: Int!` |
| `reviews` | Get all reviews | Filtering, sorting |
| `reviewsByBook` | Get reviews for a book | `bookId: Int!` |
| `reviewsByReviewer` | Get reviews by reviewer | `reviewerName: String!` |
| `reviewsByRating` | Filter by rating | `rating: Int!` |

### Mutations

| Mutation | Description | Input Type |
|----------|-------------|------------|
| `addBook` | Create a new book | `AddBookInput` |
| `updateBook` | Update existing book | `UpdateBookInput` |
| `deleteBook` | Delete a book | `id: Int!` |
| `addAuthor` | Create a new author | `AddAuthorInput` |
| `updateAuthor` | Update existing author | `UpdateAuthorInput` |
| `deleteAuthor` | Delete an author | `id: Int!` |
| `addReview` | Add a book review | `AddReviewInput` |
| `updateReview` | Update a review | `UpdateReviewInput` |
| `deleteReview` | Delete a review | `id: Int!` |
| `addGenre` | Create a new genre | `AddGenreInput` |
| `deleteGenre` | Delete a genre | `id: Int!` |

### Subscriptions

| Subscription | Description | Returns |
|--------------|-------------|---------|
| `onBookAdded` | New book added | `Book` |
| `onReviewPosted` | New review posted | `Review` |

## 🔍 Example Queries

### Get All Books with Filtering

```graphql
query GetSciFiBooks {
  books(
    where: { genres: { some: { name: { eq: "Science Fiction" } } } }
    order: { title: ASC }
  ) {
    id
    title
    author {
      name
    }
    averageRating
  }
}
```

### Add a New Book

```graphql
mutation AddNewBook {
  addBook(input: {
    title: "Dune"
    authorId: 1
    isbn: "978-0441172719"
    description: "Set in the distant future..."
    genreIds: [1]
  }) {
    book {
      id
      title
    }
    errors {
      ... on AuthorNotFoundException {
        message
        authorId
      }
    }
  }
}
```

### Subscribe to New Reviews

```graphql
subscription WatchReviews {
  onReviewPosted {
    id
    rating
    comment
    book {
      title
    }
  }
}
```

## ⚙️ Filtering & Sorting

All list queries support Hot Chocolate's built-in filtering and sorting:

### Filtering Operators

- `eq` - Equal
- `neq` - Not equal
- `contains` - String contains
- `startsWith` - String starts with
- `endsWith` - String ends with
- `in` - Value in list
- `gt`, `gte`, `lt`, `lte` - Comparisons
- `and`, `or` - Logical operators

### Example: Complex Filter

```graphql
query {
  books(
    where: {
      and: [
        { publishedDate: { gte: "1980-01-01" } }
        { pageCount: { lt: 500 } }
        { author: { nationality: { eq: "British" } } }
      ]
    }
  ) {
    title
    publishedDate
    pageCount
  }
}
```

## 🗄️ Database

### Connection String

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=booklibrary.db"
  }
}
```

### Entity Configuration

- **Authors** → Books (1:N, cascade delete)
- **Books** → Reviews (1:N, cascade delete)
- **Books** ↔ Genres (N:M, junction table `BookGenres`)
- Unique constraints on `Book.Isbn` and `Genre.Name`

## 🔧 DataLoaders

DataLoaders prevent N+1 query problems by batching database requests:

| DataLoader | Pattern | Purpose |
|------------|---------|---------|
| `AuthorByIdDataLoader` | Batch | Load authors by ID |
| `BooksByAuthorIdDataLoader` | Group | Load all books for multiple authors |
| `ReviewsByBookIdDataLoader` | Group | Load all reviews for multiple books |
| `BookByIdDataLoader` | Batch | Load books by ID |

## 🚨 Error Handling

Mutations use Hot Chocolate's mutation conventions for structured errors:

```graphql
mutation {
  addBook(input: { title: "Test", authorId: 999 }) {
    book {
      id
    }
    errors {
      ... on AuthorNotFoundException {
        message
        authorId
      }
      ... on DuplicateIsbnException {
        message
        isbn
      }
    }
  }
}
```

## 📦 Dependencies

| Package | Purpose |
|---------|---------|
| HotChocolate.AspNetCore | GraphQL server |
| HotChocolate.Data.EntityFramework | EF Core integration |
| HotChocolate.Types.Analyzers | Source generators |
| Microsoft.EntityFrameworkCore.Sqlite | SQLite provider |
