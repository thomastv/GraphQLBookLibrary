# Getting Started

A quick guide to get the GraphQL Book Library API up and running.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- IDE (one of the following):
  - Visual Studio 2022
  - VS Code with C# Dev Kit
  - JetBrains Rider

## Quick Start

### 1. Clone the Repository

```bash
git clone <repository-url>
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
cd src/BookLibrary.Api
dotnet run
```

### 5. Open GraphQL IDE

Navigate to: **https://localhost:5001/graphql/**

### 6. Run Your First Query

Paste this in the query editor and press `Ctrl+Enter`:

```graphql
query {
  books {
    id
    title
    author {
      name
    }
    averageRating
  }
}
```

## Project Structure

```
GraphQLBookLibrary/
├── src/
│   ├── BookLibrary.Domain/      # Domain entities & business logic
│   │   ├── Entities/            # Author, Book, Genre, Review
│   │   └── Exceptions/          # Custom domain exceptions
│   │
│   └── BookLibrary.Api/         # GraphQL API
│       ├── Data/                # DbContext & seed data
│       ├── GraphQL/
│       │   ├── Queries/         # GraphQL queries
│       │   ├── Mutations/       # GraphQL mutations
│       │   ├── Subscriptions/   # Real-time subscriptions
│       │   ├── DataLoaders/     # N+1 prevention
│       │   └── Types/           # Input types
│       └── Program.cs           # App configuration
│
├── tests/
│   └── BookLibrary.Domain.Tests/ # Unit tests
│
├── docs/                         # Documentation
│
├── Directory.Packages.props      # Central package management
└── GraphQLBookLibrary.slnx       # Solution file
```

## Available Commands

| Command | Description |
|---------|-------------|
| `dotnet build` | Build the solution |
| `dotnet test` | Run all unit tests |
| `dotnet run --project src/BookLibrary.Api` | Start the API |
| `dotnet watch --project src/BookLibrary.Api` | Start with hot reload |

## API Endpoints

| URL | Description |
|-----|-------------|
| `https://localhost:5001/graphql` | GraphQL endpoint |
| `https://localhost:5001/graphql/` | Nitro GraphQL IDE |
| `http://localhost:5000/graphql` | HTTP endpoint (non-secure) |

## Sample Data

The API comes pre-loaded with sample data:

- **5 Authors**: Douglas Adams, George Orwell, J.K. Rowling, Stephen King, Agatha Christie
- **6 Genres**: Science Fiction, Fantasy, Mystery, Horror, Classic, Thriller
- **12 Books**: Classic titles from each author
- **30+ Reviews**: Various ratings and comments

## Next Steps

1. **Explore the API**: Use the Nitro IDE to run queries
2. **Read the Docs**: Check other files in `/docs`
3. **Run Tests**: `dotnet test` to see all tests pass
4. **Customize**: Add your own entities, queries, and mutations

## Common Issues

### HTTPS Certificate Warning

If you see a certificate warning, trust the dev certificate:

```bash
dotnet dev-certs https --trust
```

### Port Already in Use

If port 5001 is busy, modify `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:5002;http://localhost:5003"
    }
  }
}
```

### Database Issues

Reset the database by deleting `booklibrary.db`:

```bash
del src\BookLibrary.Api\booklibrary.db
dotnet run --project src/BookLibrary.Api
```

## Getting Help

- Check the [GraphQL IDE Guide](GRAPHQL_IDE.md) for query examples
- See [Database Guide](DATABASE.md) for data management
- Review [Testing Guide](TESTING.md) for test information
- Read [Architecture Guide](ARCHITECTURE.md) for design details
