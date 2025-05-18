using FluentValidation;
using BookKeepAPI.Application.Models.BookData;

namespace BookKeepAPI.Application.Validators.BookData;

/// <summary>
/// Validator for the <see cref="Book"/> model.
/// </summary>
public class BookValidator : AbstractValidator<Book>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookValidator"/> class.
    /// </summary>
    public BookValidator()
    {
        // Include rules from BaseModelValidator
        RuleFor(x => x).SetValidator(new BaseModelValidator());

        RuleFor(book => book.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot be longer than 200 characters.");

        RuleFor(book => book.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MaximumLength(100).WithMessage("Author name cannot be longer than 100 characters.");

        RuleFor(book => book.ISBN)
            .NotEmpty().WithMessage("ISBN is required.")
            .Length(10, 13).WithMessage("ISBN must be between 10 and 13 characters.") // Simplified for common ISBN-10 and ISBN-13 lengths
            .Matches(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$").WithMessage("Invalid ISBN format.");

        RuleFor(book => book.Description)
            .MaximumLength(2000).WithMessage("Description cannot be longer than 2000 characters.")
            .When(book => !string.IsNullOrEmpty(book.Description)); // Only validate length if description is provided

        RuleFor(book => book.PublicationYear)
            .InclusiveBetween(1000, DateTime.UtcNow.Year + 5).WithMessage($"Please enter a valid publication year between 1000 and {DateTime.UtcNow.Year + 5}.") // Allow a bit into the future for upcoming books
            .When(book => book.PublicationYear.HasValue); // Only validate if publication year is provided

        RuleFor(book => book.Genre)
            .MaximumLength(50).WithMessage("Genre cannot be longer than 50 characters.")
            .When(book => !string.IsNullOrEmpty(book.Genre));

        RuleFor(book => book.CoverImageUrl)
            .MaximumLength(500).WithMessage("Cover image URL cannot be longer than 500 characters.")
            .Matches(@"^(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$").WithMessage("Please enter a valid URL for the cover image.")
            .When(book => !string.IsNullOrEmpty(book.CoverImageUrl));
    }
}