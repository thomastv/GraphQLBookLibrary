using BookLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Api.Data;

/// <summary>
/// Provides sample data seeding for the Book Library database.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Initializes the database with sample data if empty.
    /// </summary>
    /// <param name="context">The database context.</param>
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Skip if data already exists
        if (await context.Authors.AnyAsync())
            return;

        // Create genres
        var genres = CreateGenres();
        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();

        // Create authors
        var authors = CreateAuthors();
        await context.Authors.AddRangeAsync(authors);
        await context.SaveChangesAsync();

        // Create books
        var books = CreateBooks(authors, genres);
        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        // Create reviews
        var reviews = CreateReviews(books);
        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();
    }

    private static List<Genre> CreateGenres()
    {
        return
        [
            new Genre("Science Fiction", "Stories based on futuristic science and technology"),
            new Genre("Fantasy", "Stories featuring magical or supernatural elements"),
            new Genre("Mystery", "Stories involving crime solving and suspense"),
            new Genre("Horror", "Stories designed to frighten and create suspense"),
            new Genre("Classic", "Timeless literature of lasting significance"),
            new Genre("Thriller", "Fast-paced stories with excitement and suspense"),
            new Genre("Comedy", "Humorous stories designed to entertain"),
            new Genre("Dystopian", "Stories set in oppressive or post-apocalyptic societies")
        ];
    }

    private static List<Author> CreateAuthors()
    {
        return
        [
            new Author("Douglas Adams", 
                "English author, humorist, and screenwriter best known for The Hitchhiker's Guide to the Galaxy.", 
                new DateTime(1952, 3, 11), 
                "British"),
            new Author("George Orwell", 
                "English novelist, essayist, and critic famous for his works 1984 and Animal Farm.", 
                new DateTime(1903, 6, 25), 
                "British"),
            new Author("J.K. Rowling", 
                "British author best known for the Harry Potter fantasy series.", 
                new DateTime(1965, 7, 31), 
                "British"),
            new Author("Stephen King", 
                "American author of horror, supernatural fiction, suspense, and fantasy novels.", 
                new DateTime(1947, 9, 21), 
                "American"),
            new Author("Agatha Christie", 
                "English writer known for her detective novels featuring Hercule Poirot and Miss Marple.", 
                new DateTime(1890, 9, 15), 
                "British")
        ];
    }

    private static List<Book> CreateBooks(List<Author> authors, List<Genre> genres)
    {
        var sciFi = genres.First(g => g.Name == "Science Fiction");
        var fantasy = genres.First(g => g.Name == "Fantasy");
        var mystery = genres.First(g => g.Name == "Mystery");
        var horror = genres.First(g => g.Name == "Horror");
        var classic = genres.First(g => g.Name == "Classic");
        var thriller = genres.First(g => g.Name == "Thriller");
        var comedy = genres.First(g => g.Name == "Comedy");
        var dystopian = genres.First(g => g.Name == "Dystopian");

        var douglasAdams = authors.First(a => a.Name == "Douglas Adams");
        var georgeOrwell = authors.First(a => a.Name == "George Orwell");
        var jkRowling = authors.First(a => a.Name == "J.K. Rowling");
        var stephenKing = authors.First(a => a.Name == "Stephen King");
        var agathaChristie = authors.First(a => a.Name == "Agatha Christie");

        return
        [
            // Douglas Adams
            new Book
            {
                Title = "The Hitchhiker's Guide to the Galaxy",
                Isbn = "978-0345391803",
                Description = "Seconds before the Earth is demolished to make way for a galactic freeway, Arthur Dent is plucked off the planet by his friend Ford Prefect.",
                PublishedDate = new DateTime(1979, 10, 12),
                PageCount = 224,
                Publisher = "Pan Books",
                AuthorId = douglasAdams.Id,
                Author = douglasAdams,
                Genres = [sciFi, comedy]
            },
            new Book
            {
                Title = "The Restaurant at the End of the Universe",
                Isbn = "978-0345391810",
                Description = "The second book in the Hitchhiker's Guide trilogy follows the adventures of Arthur Dent and his friends.",
                PublishedDate = new DateTime(1980, 10, 1),
                PageCount = 250,
                Publisher = "Pan Books",
                AuthorId = douglasAdams.Id,
                Author = douglasAdams,
                Genres = [sciFi, comedy]
            },

            // George Orwell
            new Book
            {
                Title = "1984",
                Isbn = "978-0451524935",
                Description = "A dystopian novel set in Airstrip One, a province of the superstate Oceania in a world of perpetual war and government surveillance.",
                PublishedDate = new DateTime(1949, 6, 8),
                PageCount = 328,
                Publisher = "Secker & Warburg",
                AuthorId = georgeOrwell.Id,
                Author = georgeOrwell,
                Genres = [classic, dystopian, sciFi]
            },
            new Book
            {
                Title = "Animal Farm",
                Isbn = "978-0451526342",
                Description = "An allegorical novella reflecting events leading up to the Russian Revolution and the Stalinist era of the Soviet Union.",
                PublishedDate = new DateTime(1945, 8, 17),
                PageCount = 112,
                Publisher = "Secker & Warburg",
                AuthorId = georgeOrwell.Id,
                Author = georgeOrwell,
                Genres = [classic, dystopian]
            },

            // J.K. Rowling
            new Book
            {
                Title = "Harry Potter and the Philosopher's Stone",
                Isbn = "978-0747532699",
                Description = "Harry Potter discovers on his 11th birthday that he is the orphaned son of two powerful wizards and possesses magical powers of his own.",
                PublishedDate = new DateTime(1997, 6, 26),
                PageCount = 223,
                Publisher = "Bloomsbury",
                AuthorId = jkRowling.Id,
                Author = jkRowling,
                Genres = [fantasy]
            },
            new Book
            {
                Title = "Harry Potter and the Chamber of Secrets",
                Isbn = "978-0747538493",
                Description = "Harry's second year at Hogwarts is filled with fresh horrors, including an ancient prophecy of doom.",
                PublishedDate = new DateTime(1998, 7, 2),
                PageCount = 251,
                Publisher = "Bloomsbury",
                AuthorId = jkRowling.Id,
                Author = jkRowling,
                Genres = [fantasy]
            },
            new Book
            {
                Title = "Harry Potter and the Prisoner of Azkaban",
                Isbn = "978-0747546290",
                Description = "Harry's third year involves a dangerous prisoner who has escaped from Azkaban, the wizard prison.",
                PublishedDate = new DateTime(1999, 7, 8),
                PageCount = 317,
                Publisher = "Bloomsbury",
                AuthorId = jkRowling.Id,
                Author = jkRowling,
                Genres = [fantasy]
            },

            // Stephen King
            new Book
            {
                Title = "The Shining",
                Isbn = "978-0385121675",
                Description = "Jack Torrance's new job as the winter caretaker of the isolated Overlook Hotel leads to a supernatural descent into madness.",
                PublishedDate = new DateTime(1977, 1, 28),
                PageCount = 447,
                Publisher = "Doubleday",
                AuthorId = stephenKing.Id,
                Author = stephenKing,
                Genres = [horror, thriller]
            },
            new Book
            {
                Title = "It",
                Isbn = "978-1501142970",
                Description = "Seven adults return to their hometown to confront a nightmare they first encountered as teenagers: a murderous clown called Pennywise.",
                PublishedDate = new DateTime(1986, 9, 15),
                PageCount = 1138,
                Publisher = "Viking",
                AuthorId = stephenKing.Id,
                Author = stephenKing,
                Genres = [horror]
            },

            // Agatha Christie
            new Book
            {
                Title = "Murder on the Orient Express",
                Isbn = "978-0007119318",
                Description = "Detective Hercule Poirot investigates the murder of an American tycoon aboard the famous train.",
                PublishedDate = new DateTime(1934, 1, 1),
                PageCount = 256,
                Publisher = "Collins Crime Club",
                AuthorId = agathaChristie.Id,
                Author = agathaChristie,
                Genres = [mystery, classic]
            },
            new Book
            {
                Title = "And Then There Were None",
                Isbn = "978-0062073488",
                Description = "Ten strangers are lured to an isolated island mansion where they are accused of getting away with murder.",
                PublishedDate = new DateTime(1939, 11, 6),
                PageCount = 272,
                Publisher = "Collins Crime Club",
                AuthorId = agathaChristie.Id,
                Author = agathaChristie,
                Genres = [mystery, thriller, classic]
            }
        ];
    }

    private static List<Review> CreateReviews(List<Book> books)
    {
        var reviews = new List<Review>();
        var reviewerNames = new[] { "Alice Johnson", "Bob Smith", "Charlie Brown", "Diana Ross", "Edward Norton", 
                                     "Fiona Apple", "George Lucas", "Hannah Montana", "Ivan Drago", "Julia Roberts" };
        
        var comments = new Dictionary<int, string[]>
        {
            { 5, ["Absolutely amazing!", "A masterpiece!", "Couldn't put it down!", "One of the best books I've ever read!", "Highly recommended!"] },
            { 4, ["Really enjoyed this one.", "Great read!", "Well written and engaging.", "Almost perfect!", "Very entertaining."] },
            { 3, ["It was okay.", "Decent read.", "Had its moments.", "Not bad, not great.", "Average but enjoyable."] },
            { 2, ["Expected more.", "Disappointing.", "Struggled to finish.", "Not my cup of tea.", "Could have been better."] },
            { 1, ["Didn't enjoy it at all.", "Not recommended.", "Waste of time.", "Very disappointing.", "Couldn't finish it."] }
        };

        var random = new Random(42); // Fixed seed for reproducible results
        var reviewId = 1;

        foreach (var book in books)
        {
            var reviewCount = random.Next(2, 6); // 2-5 reviews per book
            var usedReviewers = new HashSet<string>();

            for (var i = 0; i < reviewCount; i++)
            {
                // Get unique reviewer
                string reviewer;
                do
                {
                    reviewer = reviewerNames[random.Next(reviewerNames.Length)];
                } while (usedReviewers.Contains(reviewer));
                usedReviewers.Add(reviewer);

                // Weight ratings towards higher values (more realistic)
                var ratingRoll = random.Next(100);
                var rating = ratingRoll switch
                {
                    < 5 => 1,   // 5% chance
                    < 15 => 2,  // 10% chance
                    < 35 => 3,  // 20% chance
                    < 65 => 4,  // 30% chance
                    _ => 5      // 35% chance
                };

                var comment = comments[rating][random.Next(comments[rating].Length)];
                var daysAgo = random.Next(1, 365);

                reviews.Add(new Review
                {
                    Id = reviewId++,
                    Rating = rating,
                    Comment = comment,
                    ReviewerName = reviewer,
                    ReviewerEmail = $"{reviewer.ToLower().Replace(" ", ".")}@email.com",
                    BookId = book.Id,
                    Book = book,
                    CreatedAt = DateTime.UtcNow.AddDays(-daysAgo)
                });
            }
        }

        return reviews;
    }
}
