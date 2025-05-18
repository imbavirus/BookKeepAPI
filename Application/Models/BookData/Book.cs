using System.ComponentModel.DataAnnotations;

namespace BookKeepAPI.Application.Models.BookData;

/// <summary>
/// Represents the interface for a book model with properties such as title, author, ISBN, and more.
/// </summary>
public interface IBook : IBaseModel
{
    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    string Title { get; set; }
    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    string Author { get; set; }
    /// <summary>
    /// Gets or sets the International Standard Book Number (ISBN) of the book.
    /// </summary>
    string ISBN { get; set; }
    /// <summary>
    /// Gets or sets the description or synopsis of the book. This can be null if no description is available.
    /// </summary>
    string? Description { get; set; }
    /// <summary>
    /// Gets or sets the year the book was published. This can be null if the publication year is unknown.
    /// </summary>
    int? PublicationYear { get; set; }
    /// <summary>
    /// Gets or sets the genre(s) of the book. This can be null if the genre is not specified.
    /// </summary>
    string? Genre { get; set; }
    /// <summary>
    /// Gets or sets the URL for the book's cover image. This can be null if no cover image URL is available.
    /// </summary>
    string? CoverImageUrl { get; set; }
}

/// <summary>
/// Represents a book with details such as title, author, ISBN, description, publication year, genre, and cover image URL.
/// </summary>
/// <param name="title">The title of the book.</param>
/// <param name="author">The author of the book.</param>
/// <param name="isbn">The ISBN of the book.</param>
/// <param name="description">The description of the book.</param>
/// <param name="publicationYear">The publication year of the book.</param>
/// <param name="genre">The genre of the book.</param>
/// <param name="coverImageUrl">The URL of the book's cover image.</param>
public class Book(string title, string author, string isbn, string? description = null, int? publicationYear = null, string? genre = null, string? coverImageUrl = null) : BaseModel, IBook
{
    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters.")]
    public string Title { get; set; } = title;

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    [Required(ErrorMessage = "Author is required.")]
    [StringLength(100, ErrorMessage = "Author name cannot be longer than 100 characters.")]
    public string Author { get; set; } = author;

    /// <summary>
    /// Gets or sets the International Standard Book Number (ISBN) of the book.
    /// </summary>
    [Required(ErrorMessage = "ISBN is required.")]
    [RegularExpression(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$", ErrorMessage = "Invalid ISBN format.")]
    [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 13 characters.")]
    public string ISBN { get; set; } = isbn;

    /// <summary>
    /// Gets or sets the description or synopsis of the book.
    /// This property is optional.
    /// </summary>
    [StringLength(2000, ErrorMessage = "Description cannot be longer than 2000 characters.")]
    public string? Description { get; set; } = description;

    /// <summary>
    /// Gets or sets the year the book was published.
    /// This property is optional.
    /// </summary>
    [Range(1000, 9999, ErrorMessage = "Please enter a valid publication year.")]
    public int? PublicationYear { get; set; } = publicationYear;

    /// <summary>
    /// Gets or sets the genre(s) of the book.
    /// This property is optional.
    /// </summary>
    [StringLength(50, ErrorMessage = "Genre cannot be longer than 50 characters.")]
    public string? Genre { get; set; } = genre;

    /// <summary>
    /// Gets or sets the URL for the book's cover image.
    /// This property is optional and should be a valid URL.
    /// </summary>
    [Url(ErrorMessage = "Please enter a valid URL for the cover image.")]
    [StringLength(500, ErrorMessage = "Cover image URL cannot be longer than 500 characters.")]
    public string? CoverImageUrl { get; set; } = coverImageUrl;
}
