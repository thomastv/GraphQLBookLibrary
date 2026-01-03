# GraphQL Book Library API

A comprehensive GraphQL API for managing a book library, built with .NET 10 and Hot Chocolate.

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        GraphQL Clients                          │
│              (Banana Cake Pop, Apollo, Postman)                 │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                     BookLibrary.Api                             │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐       │
│  │    Queries    │  │   Mutations   │  │ Subscriptions │       │
│  │ (Books,       │  │ (Add, Update, │  │ (BookAdded,   │       │
│  │  Authors,     │  │  Delete)      │  │  ReviewPosted)│       │
│  │  Reviews)     │  │               │  │               │       │
│  └───────────────┘  └───────────────┘  └───────────────┘       │
│           │                 │                  │                │
│           ▼                 ▼                  ▼                │
│  ┌─────────────────────────────────────────────────────┐       │
│  │              DataLoaders (N+1 Prevention)           │       │
│  └─────────────────────────────────────────────────────┘       │
│                             │                                   │
│                             ▼                                   │
│  ┌─────────────────────────────────────────────────────┐       │
│  │           Entity Framework Core (SQLite)            │       │
│  └─────────────────────────────────────────────────────┘       │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                    BookLibrary.Domain                           │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐   │
│  │   Book    │  │  Author   │  │  Review   │  │   Genre   │   │
│  └───────────┘  └───────────┘  └───────────┘  └───────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- IDE: Visual Studio 2022, VS Code with C# Dev Kit, or JetBrains Rider

## 🚀 Getting Started

### 1. Clone and Navigate

```bash
cd GraphQLBookLibrary
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run the API

```bash
dotnet run --project src/BookLibrary.Api
```

### 5. Access GraphQL Playground

Open your browser and navigate to:
- **Banana Cake Pop (GraphQL IDE)**: https://localhost:5001/graphql/

## 📁 Project Structure

| Project | Description |
|---------|-------------|
| `BookLibrary.Domain` | Domain entities, value objects, and business rules |
| `BookLibrary.Api` | GraphQL API with Hot Chocolate, EF Core data access |
| `BookLibrary.Domain.Tests` | Unit tests for domain layer |

## 🔍 Sample GraphQL Queries

### Get All Books with Authors

```graphql
query {
  books {
    id
    title
    isbn
    publishedDate
    author {
      name
    }
    genres {
      name
    }
    averageRating
  }
}
```

### Get Book by ID with Reviews

```graphql
query {
  book(id: 1) {
    title
    description
    author {
      name
      biography
    }
    reviews {
      rating
      comment
      reviewerName
      createdAt
    }
  }
}
```

### Filter Books by Genre

```graphql
query {
  books(where: { genres: { some: { name: { eq: "Science Fiction" } } } }) {
    title
    author {
      name
    }
  }
}
```

### Sort Books by Rating

```graphql
query {
  books(order: { averageRating: DESC }) {
    title
    averageRating
  }
}
```

### Add a New Book (Mutation)

```graphql
mutation {
  addBook(input: {
    title: "New Book Title"
    isbn: "978-1234567890"
    description: "A great new book"
    authorId: 1
    genreIds: [1, 2]
  }) {
    book {
      id
      title
    }
    errors {
      ... on AuthorNotFoundException {
        message
      }
    }
  }
}
```

### Add a Review (Mutation)

```graphql
mutation {
  addReview(input: {
    bookId: 1
    rating: 5
    comment: "Excellent book!"
    reviewerName: "John Doe"
  }) {
    review {
      id
      rating
      comment
    }
  }
}
```

### Subscribe to New Books

```graphql
subscription {
  onBookAdded {
    id
    title
    author {
      name
    }
  }
}
```

## 🧪 Running Tests

### Run All Tests

```bash
dotnet test
```

### Run with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test Project

```bash
dotnet test tests/BookLibrary.Domain.Tests
```

## 📊 Database

The API uses **SQLite** for data persistence:

- **Development**: File-based SQLite (`booklibrary.db`)
- **Testing**: In-memory SQLite for fast, isolated tests

### Database Location

The database file is created in the API project directory:
```
src/BookLibrary.Api/booklibrary.db
```

### Sample Data

The application seeds sample data on first run:
- **5 Authors**: Douglas Adams, George Orwell, J.K. Rowling, Stephen King, Agatha Christie
- **6 Genres**: Science Fiction, Fantasy, Mystery, Horror, Classic, Thriller
- **12 Books**: Classic titles from each author
- **25+ Reviews**: Various ratings and comments

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=booklibrary.db"
  },
  "GraphQL": {
    "EnableIntrospection": true,
    "MaxExecutionDepth": 15
  }
}
```

## 📚 Key Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Runtime |
| Hot Chocolate | 15.1.2 | GraphQL Server |
| Entity Framework Core | 9.0.0 | ORM |
| SQLite | - | Database |
| xUnit | 2.9.2 | Testing Framework |
| FluentAssertions | 7.0.0 | Test Assertions |

## 📖 Additional Documentation

- [BookLibrary.Domain README](src/BookLibrary.Domain/README.md) - Domain model documentation
- [BookLibrary.Api README](src/BookLibrary.Api/README.md) - API and GraphQL schema documentation
- [Domain Tests README](tests/BookLibrary.Domain.Tests/README.md) - Domain testing guide
- [API Tests README](tests/BookLibrary.Api.Tests/README.md) - GraphQL testing guide
## 📄 License

This project is licensed under the MIT License.
