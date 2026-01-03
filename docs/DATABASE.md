# Database Guide

This guide covers database configuration, seeding, and migrations for the GraphQL Book Library project.

## Overview

The project uses:
- **Entity Framework Core 9.0** as the ORM
- **SQLite** as the database provider
- **Code-First** approach with automatic migrations

## Database Location

### Development
- File: `src/BookLibrary.Api/booklibrary.db`
- Connection String: `Data Source=booklibrary.db`

### Testing
- In-memory SQLite database
- Created fresh for each test run

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=booklibrary.db"
  }
}
```

### DbContext Registration

In `Program.cs`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## Database Schema

### Entity Relationship Diagram

```
┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│   Author    │       │    Book     │       │   Review    │
├─────────────┤       ├─────────────┤       ├─────────────┤
│ Id (PK)     │──┐    │ Id (PK)     │──┐    │ Id (PK)     │
│ Name        │  │    │ Title       │  │    │ Rating      │
│ Biography   │  │    │ Isbn        │  │    │ Comment     │
│ DateOfBirth │  │    │ Description │  │    │ ReviewerName│
│ Nationality │  └───<│ AuthorId(FK)│  └───<│ BookId (FK) │
│ ImageUrl    │       │ PublishedDt │       │ ReviewerEmail│
└─────────────┘       │ PageCount   │       │ CreatedAt   │
                      │ CoverImgUrl │       │ UpdatedAt   │
                      │ Language    │       └─────────────┘
                      │ Publisher   │
                      └──────┬──────┘
                             │
                             │ Many-to-Many
                             │
                      ┌──────┴──────┐
                      │ BookGenre   │
                      │ (Junction)  │
                      ├─────────────┤
                      │ BookId (FK) │
                      │ GenreId(FK) │
                      └──────┬──────┘
                             │
                      ┌──────┴──────┐
                      │   Genre     │
                      ├─────────────┤
                      │ Id (PK)     │
                      │ Name        │
                      │ Description │
                      └─────────────┘
```

### Tables

| Table | Description |
|-------|-------------|
| `Authors` | Book authors |
| `Books` | Library books |
| `Genres` | Book categories |
| `Reviews` | Book reviews |
| `BookGenre` | Junction table for Book-Genre many-to-many |

## Data Seeding

The database is automatically seeded with sample data on first run.

### Seed Data Contents

| Entity | Count | Examples |
|--------|-------|----------|
| Authors | 5 | Douglas Adams, George Orwell, J.K. Rowling, Stephen King, Agatha Christie |
| Genres | 6 | Science Fiction, Fantasy, Mystery, Horror, Classic, Thriller |
| Books | 12 | The Hitchhiker's Guide to the Galaxy, 1984, Harry Potter, The Shining, etc. |
| Reviews | 30+ | Various ratings and comments |

### Seed Data Location

The seed data is defined in:
- `src/BookLibrary.Api/Data/SeedData.cs`

### How Seeding Works

```csharp
// In Program.cs
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    await SeedData.InitializeAsync(context);
}
```

### Customizing Seed Data

Edit `SeedData.cs` to modify initial data:

```csharp
public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (await context.Authors.AnyAsync())
            return; // Already seeded

        // Add your custom seed data here
        var authors = new List<Author>
        {
            new Author("Your Author", "Biography", null, "Nationality"),
            // ...
        };

        context.Authors.AddRange(authors);
        await context.SaveChangesAsync();
    }
}
```

## Migrations

### Enable Migrations

First, ensure you have the EF Core tools installed:

```bash
dotnet tool install --global dotnet-ef
```

### Create Initial Migration

```bash
cd src/BookLibrary.Api
dotnet ef migrations add InitialCreate
```

### Apply Migrations

```bash
dotnet ef database update
```

### Create New Migration (After Model Changes)

```bash
dotnet ef migrations add AddNewFeature
dotnet ef database update
```

### List Migrations

```bash
dotnet ef migrations list
```

### Remove Last Migration

```bash
dotnet ef migrations remove
```

### Generate SQL Script

```bash
dotnet ef migrations script -o migration.sql
```

### Revert to Specific Migration

```bash
dotnet ef database update MigrationName
```

### Reset Database

```bash
# Delete and recreate
dotnet ef database drop --force
dotnet ef database update
```

Or simply delete the `booklibrary.db` file and restart the application.

## AppDbContext

Located at `src/BookLibrary.Api/Data/AppDbContext.cs`:

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Genres)
            .WithMany(g => g.Books);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId);
    }
}
```

## Working with Data

### Query Examples

```csharp
// Get all books with authors
var books = await context.Books
    .Include(b => b.Author)
    .ToListAsync();

// Get book by ID with all related data
var book = await context.Books
    .Include(b => b.Author)
    .Include(b => b.Genres)
    .Include(b => b.Reviews)
    .FirstOrDefaultAsync(b => b.Id == id);

// Get books by author
var authorBooks = await context.Books
    .Where(b => b.Author.Name.Contains("Orwell"))
    .ToListAsync();

// Get top-rated books
var topBooks = await context.Books
    .Where(b => b.Reviews.Any())
    .OrderByDescending(b => b.Reviews.Average(r => r.Rating))
    .Take(10)
    .ToListAsync();
```

### Insert Examples

```csharp
// Add new author
var author = new Author("New Author");
context.Authors.Add(author);
await context.SaveChangesAsync();

// Add book with author
var book = new Book("New Book")
{
    AuthorId = author.Id,
    Isbn = "978-1234567890"
};
context.Books.Add(book);
await context.SaveChangesAsync();
```

### Update Examples

```csharp
var book = await context.Books.FindAsync(id);
if (book != null)
{
    book.Title = "Updated Title";
    await context.SaveChangesAsync();
}
```

### Delete Examples

```csharp
var book = await context.Books.FindAsync(id);
if (book != null)
{
    context.Books.Remove(book);
    await context.SaveChangesAsync();
}
```

## Database Maintenance

### View Database Contents

Use a SQLite browser tool:
- [DB Browser for SQLite](https://sqlitebrowser.org/)
- [SQLite Viewer VS Code Extension](https://marketplace.visualstudio.com/items?itemName=qwtel.sqlite-viewer)

### Backup Database

```bash
copy src\BookLibrary.Api\booklibrary.db src\BookLibrary.Api\booklibrary.db.backup
```

### Query Database Directly

```bash
sqlite3 src/BookLibrary.Api/booklibrary.db
```

```sql
-- List all tables
.tables

-- Show table schema
.schema Books

-- Query data
SELECT * FROM Books;
SELECT * FROM Authors;

-- Exit
.exit
```

## Performance Considerations

### 1. Use Projections

Instead of loading entire entities:

```csharp
// Good - only loads needed fields
var bookTitles = await context.Books
    .Select(b => new { b.Id, b.Title })
    .ToListAsync();

// Avoid - loads entire entity
var books = await context.Books.ToListAsync();
```

### 2. Use AsNoTracking for Read-Only

```csharp
var books = await context.Books
    .AsNoTracking()
    .ToListAsync();
```

### 3. Eager Loading vs Lazy Loading

Hot Chocolate with DataLoaders handles this efficiently, but for direct queries:

```csharp
// Eager loading - single query with joins
var books = await context.Books
    .Include(b => b.Author)
    .Include(b => b.Reviews)
    .ToListAsync();
```

### 4. Pagination

Always paginate large result sets:

```csharp
var books = await context.Books
    .Skip(page * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

## Troubleshooting

### Database Locked Error

SQLite can only handle one write operation at a time. If you see "database is locked":

1. Close any other applications using the database
2. Restart the API
3. Delete `.db-wal` and `.db-shm` files if present

### Migration Errors

If migrations fail:

```bash
# Remove the failed migration
dotnet ef migrations remove

# Check for model errors
dotnet build

# Try again
dotnet ef migrations add FixedMigration
```

### Reset Everything

```bash
# Delete database and migrations
del src\BookLibrary.Api\booklibrary.db
del src\BookLibrary.Api\Migrations\* 

# Create fresh migration
dotnet ef migrations add InitialCreate

# Start application (will create DB and seed)
dotnet run --project src/BookLibrary.Api
```
