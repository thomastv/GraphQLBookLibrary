using BookLibrary.Domain.Entities;

namespace BookLibrary.Api.GraphQL.Subscriptions;

/// <summary>
/// GraphQL subscriptions for real-time book-related events.
/// </summary>
[SubscriptionType]
public static class BookSubscriptions
{
    /// <summary>
    /// Subscribes to new book additions.
    /// </summary>
    /// <param name="book">The newly added book.</param>
    /// <returns>The book that was added.</returns>
    [Subscribe]
    [Topic]
    public static Book OnBookAdded([EventMessage] Book book) => book;

    /// <summary>
    /// Subscribes to new review postings.
    /// </summary>
    /// <param name="review">The newly posted review.</param>
    /// <returns>The review that was posted.</returns>
    [Subscribe]
    [Topic]
    public static Review OnReviewPosted([EventMessage] Review review) => review;
}
