

namespace BookKeepAPI.Application.Managers.External.OpenLibrary.Implementation;

/// <summary>
/// Implements <see cref="IOpenLibraryManager"/> to fetch book cover images from the Open Library API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OpenLibraryManager"/> class.
/// </remarks>
/// <param name="httpClientFactory">The HTTP client factory for creating HTTP clients.</param>
public class OpenLibraryManager(IHttpClientFactory httpClientFactory) : IOpenLibraryManager
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

    /// <inheritdoc />
    public async Task<string?> FetchCoverUrlByIsbnAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return null;

        var client = _httpClientFactory.CreateClient("OpenLibraryClient"); // Using a named client is good practice
        string coverUrl = $"http://covers.openlibrary.org/b/isbn/{isbn}-L.jpg";

        var response = await client.GetAsync(coverUrl, HttpCompletionOption.ResponseHeadersRead);

        if (response.IsSuccessStatusCode &&
            response.Content.Headers.ContentType?.MediaType?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) == true)
        {
            return coverUrl;
        }
        return null;
    }
}