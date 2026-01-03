# Documentation

Welcome to the GraphQL Book Library documentation.

## Quick Links

| Document | Description |
|----------|-------------|
| [Getting Started](GETTING_STARTED.md) | Quick setup and first steps |
| [GraphQL IDE (Nitro)](GRAPHQL_IDE.md) | How to use the GraphQL playground |
| [Database Guide](DATABASE.md) | Database setup, seeding, and migrations |
| [Testing Guide](TESTING.md) | Running and writing tests |
| [Architecture](ARCHITECTURE.md) | Design patterns and project structure |

## What is GraphQL?

GraphQL is a query language for APIs that allows clients to request exactly the data they need. Unlike REST, where you get fixed data structures, GraphQL lets you specify your data requirements.

### REST vs GraphQL

**REST** - Multiple endpoints, fixed responses:
```
GET /api/books          → All book fields
GET /api/books/1        → Single book, all fields
GET /api/books/1/author → Author data
```

**GraphQL** - Single endpoint, flexible queries:
```graphql
query {
  book(id: 1) {
    title           # Only get title
    author {
      name          # Only get author name
    }
  }
}
```

## Key Features

### 1. Queries
Fetch data from the server:
```graphql
query {
  books {
    title
    averageRating
  }
}
```

### 2. Mutations
Create, update, or delete data:
```graphql
mutation {
  addBook(input: { title: "New Book", authorId: 1 }) {
    book { id }
  }
}
```

### 3. Subscriptions
Real-time updates:
```graphql
subscription {
  onBookAdded {
    title
  }
}
```

### 4. Filtering
Query with conditions:
```graphql
query {
  books(where: { rating: { gte: 4 } }) {
    title
  }
}
```

### 5. Sorting
Order results:
```graphql
query {
  books(order: { title: ASC }) {
    title
  }
}
```

### 6. Pagination
Handle large datasets:
```graphql
query {
  books(first: 10, after: "cursor") {
    edges {
      node { title }
    }
    pageInfo { hasNextPage }
  }
}
```

## Technology Stack

| Technology | Purpose | Version |
|------------|---------|---------|
| .NET | Runtime | 10.0 |
| Hot Chocolate | GraphQL Server | 15.1.2 |
| Entity Framework Core | ORM | 9.0.0 |
| SQLite | Database | - |
| xUnit | Testing | 2.9.2 |
| FluentAssertions | Test Assertions | 7.0.0 |

## API Access

- **GraphQL Endpoint**: `https://localhost:5001/graphql`
- **GraphQL IDE**: `https://localhost:5001/graphql/`

## Getting Help

1. **Explore the Schema**: Use the Schema tab in Nitro
2. **Check Documentation**: See the introspection queries
3. **Run Sample Queries**: Copy examples from [GraphQL IDE Guide](GRAPHQL_IDE.md)
