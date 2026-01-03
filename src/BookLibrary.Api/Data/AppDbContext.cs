using BookLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.Data;

/// <summary>
/// Entity Framework Core database context for the Book Library application.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Creates a new instance of <see cref="AppDbContext"/>.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Authors table.
    /// </summary>
    public DbSet<Author> Authors => Set<Author>();

    /// <summary>
    /// Gets or sets the Books table.
    /// </summary>
    public DbSet<Book> Books => Set<Book>();

    /// <summary>
    /// Gets or sets the Genres table.
    /// </summary>
    public DbSet<Genre> Genres => Set<Genre>();

    /// <summary>
    /// Gets or sets the Reviews table.
    /// </summary>
    public DbSet<Review> Reviews => Set<Review>();

    /// <summary>
    /// Configures the entity mappings and relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Author configuration
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Biography).HasMaxLength(2000);
            entity.Property(e => e.Nationality).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            
            entity.HasMany(e => e.Books)
                  .WithOne(b => b.Author)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Book configuration
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Isbn).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Language).HasMaxLength(50).HasDefaultValue("English");
            entity.Property(e => e.Publisher).HasMaxLength(200);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);

            entity.HasIndex(e => e.Isbn).IsUnique();
            
            entity.HasMany(e => e.Genres)
                  .WithMany(g => g.Books)
                  .UsingEntity(j => j.ToTable("BookGenres"));

            entity.HasMany(e => e.Reviews)
                  .WithOne(r => r.Book)
                  .HasForeignKey(r => r.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Ignore computed properties
            entity.Ignore(e => e.AverageRating);
            entity.Ignore(e => e.ReviewCount);
        });

        // Genre configuration
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Review configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Comment).HasMaxLength(2000);
            entity.Property(e => e.ReviewerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ReviewerEmail).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");

            entity.HasIndex(e => e.BookId);
            entity.HasIndex(e => e.Rating);
        });
    }
}
