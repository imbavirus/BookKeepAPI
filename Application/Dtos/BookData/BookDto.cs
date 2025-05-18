namespace BookKeepAPI.Application.Dtos.BookData;

/// <summary>
/// Data Transfer Object for Book information.
/// This DTO represents a book entity, excluding its base model properties.
/// </summary>
public class BookDto
{
    /// <summary>
    /// Gets or sets the unique GUID of the book.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    public string Author { get; set; } = default!;

    /// <summary>
    /// Gets or sets the International Standard Book Number (ISBN) of the book.
    /// </summary>
    public string ISBN { get; set; } = default!;

    /// <summary>
    /// Gets or sets the description of the book.
    /// This field is optional.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the publication year of the book.
    /// This field is optional.
    /// </summary>
    public int? PublicationYear { get; set; }

    /// <summary>
    /// Gets or sets the genre of the book.
    /// This field is optional.
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// Gets or sets the URL for the book's cover image.
    /// This field is optional.
    /// </summary>
    public string? CoverImageUrl { get; set; }
}