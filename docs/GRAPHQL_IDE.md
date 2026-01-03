# GraphQL IDE (Nitro)

Hot Chocolate includes **Nitro**, a powerful GraphQL IDE for exploring and testing your API.

## Accessing Nitro

Once the API is running, open your browser and navigate to:

```
https://localhost:5001/graphql/
```

## Interface Overview

### Left Panel - Schema Explorer
- **Schema**: Browse all types, queries, mutations, and subscriptions
- **History**: View previously executed queries
- **Collections**: Save and organize frequently used queries

### Center Panel - Query Editor
- Write GraphQL queries with syntax highlighting
- Auto-completion for fields and types
- Multi-tab support for multiple queries

### Right Panel - Results
- View query results in JSON format
- Error messages with detailed information
- Execution time statistics

### Bottom Panel - Variables & Headers
- **Variables**: Define query variables in JSON format
- **Headers**: Add custom HTTP headers (e.g., Authorization)

## Sample Queries

### Get All Books

```graphql
query {
  books {
    id
    title
    isbn
    description
    publishedDate
    averageRating
    reviewCount
  }
}
```

### Get Book with Author and Reviews

```graphql
query {
  books {
    id
    title
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

### Get Single Book by ID

```graphql
query GetBook($id: Int!) {
  bookById(id: $id) {
    id
    title
    description
    author {
      name
    }
  }
}
```

**Variables Panel:**
```json
{
  "id": 1
}
```

### Get All Authors with Their Books

```graphql
query {
  authors {
    id
    name
    biography
    nationality
    bookCount
    books {
      title
      averageRating
    }
  }
}
```

### Get All Genres

```graphql
query {
  genres {
    id
    name
    description
    books {
      title
    }
  }
}
```

### Get Reviews with High Ratings

```graphql
query {
  reviews(where: { rating: { gte: 4 } }) {
    id
    rating
    comment
    reviewerName
    book {
      title
    }
  }
}
```

## Filtering

Use the `where` argument to filter results:

```graphql
# Books by specific author
query {
  books(where: { author: { name: { contains: "Orwell" } } }) {
    title
    author {
      name
    }
  }
}

# Books with rating above 4
query {
  books(where: { averageRating: { gte: 4.0 } }) {
    title
    averageRating
  }
}
```

### Available Filter Operations

| Operation | Description | Example |
|-----------|-------------|---------|
| `eq` | Equals | `{ title: { eq: "1984" } }` |
| `neq` | Not equals | `{ rating: { neq: 1 } }` |
| `contains` | Contains substring | `{ name: { contains: "King" } }` |
| `startsWith` | Starts with | `{ title: { startsWith: "The" } }` |
| `endsWith` | Ends with | `{ title: { endsWith: "Ring" } }` |
| `gt` | Greater than | `{ rating: { gt: 3 } }` |
| `gte` | Greater than or equal | `{ rating: { gte: 4 } }` |
| `lt` | Less than | `{ rating: { lt: 3 } }` |
| `lte` | Less than or equal | `{ rating: { lte: 2 } }` |
| `in` | In list | `{ id: { in: [1, 2, 3] } }` |
| `nin` | Not in list | `{ id: { nin: [1, 2] } }` |

## Sorting

Use the `order` argument to sort results:

```graphql
# Books sorted by title ascending
query {
  books(order: { title: ASC }) {
    title
  }
}

# Books sorted by average rating descending
query {
  books(order: { averageRating: DESC }) {
    title
    averageRating
  }
}

# Multiple sort criteria
query {
  books(order: [{ author: { name: ASC } }, { title: ASC }]) {
    title
    author {
      name
    }
  }
}
```

## Pagination

Use cursor-based pagination for large datasets:

```graphql
query {
  books(first: 5) {
    edges {
      node {
        id
        title
      }
      cursor
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
      startCursor
      endCursor
    }
    totalCount
  }
}
```

### Get Next Page

```graphql
query {
  books(first: 5, after: "CURSOR_FROM_PREVIOUS_QUERY") {
    edges {
      node {
        id
        title
      }
    }
    pageInfo {
      hasNextPage
      endCursor
    }
  }
}
```

## Mutations

### Add a New Author

```graphql
mutation {
  addAuthor(input: {
    name: "Isaac Asimov"
    biography: "American writer and professor of biochemistry"
    nationality: "American"
  }) {
    author {
      id
      name
    }
    errors {
      __typename
      ... on Error {
        message
      }
    }
  }
}
```

### Add a New Book

```graphql
mutation {
  addBook(input: {
    title: "Foundation"
    isbn: "978-0553293357"
    description: "The first novel in Isaac Asimov's classic science-fiction masterpiece"
    authorId: 1
    genreIds: [1]
  }) {
    book {
      id
      title
      author {
        name
      }
    }
    errors {
      __typename
    }
  }
}
```

### Add a Review

```graphql
mutation {
  addReview(input: {
    bookId: 1
    rating: 5
    comment: "Absolutely brilliant!"
    reviewerName: "John Doe"
    reviewerEmail: "john@example.com"
  }) {
    review {
      id
      rating
      comment
    }
    errors {
      __typename
    }
  }
}
```

### Update a Book

```graphql
mutation {
  updateBook(input: {
    id: 1
    title: "Updated Title"
    description: "Updated description"
  }) {
    book {
      id
      title
      description
    }
    errors {
      __typename
    }
  }
}
```

### Delete a Book

```graphql
mutation {
  deleteBook(id: 1) {
    success
    errors {
      __typename
    }
  }
}
```

## Subscriptions

Subscriptions allow real-time updates. Open a new tab in Nitro for subscriptions:

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

### Subscribe to New Reviews

```graphql
subscription {
  onReviewAdded {
    id
    rating
    comment
    book {
      title
    }
  }
}
```

## Tips & Tricks

### 1. Use Fragments for Reusable Fields

```graphql
fragment BookDetails on Book {
  id
  title
  isbn
  averageRating
}

query {
  books {
    ...BookDetails
    author {
      name
    }
  }
}
```

### 2. Use Variables for Dynamic Queries

Instead of hardcoding values, use variables:

```graphql
query GetBooksByAuthor($authorName: String!) {
  books(where: { author: { name: { contains: $authorName } } }) {
    title
  }
}
```

### 3. Introspection Query

Get the full schema:

```graphql
query {
  __schema {
    types {
      name
      description
    }
  }
}
```

### 4. Get Type Details

```graphql
query {
  __type(name: "Book") {
    name
    fields {
      name
      type {
        name
      }
    }
  }
}
```

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl + Enter` | Execute query |
| `Ctrl + Space` | Trigger autocomplete |
| `Ctrl + /` | Toggle comment |
| `Ctrl + D` | Duplicate line |
| `Ctrl + Shift + F` | Format query |

## Exporting Queries

1. Click the **Collections** tab
2. Create a new collection
3. Save queries with names and descriptions
4. Export collections as JSON for sharing
