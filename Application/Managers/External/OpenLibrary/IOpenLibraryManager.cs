namespace BookKeepAPI.Application.Managers.External.OpenLibrary;

/// <summary>
/// Defines the contract for a helper that interacts with the Open Library API,
/// specifically for fetching book cover images.
/// </summary>
public interface IOpenLibraryManager
{
    /// <summary>
    /// Attempts to fetch a book cover URL from the Open Library Covers API using the ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN of the book.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the URL of the large cover image if found and valid; otherwise, null.</returns>
    Task<string?> FetchCoverUrlByIsbnAsync(string isbn);
}
