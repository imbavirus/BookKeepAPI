using System;
using BookKeepAPI.Application.Dtos.BookData;
using FluentValidation;

namespace BookKeepAPI.Application.Validators.BookData
{
    /// <summary>
    /// Validator for the <see cref="BookDto"/> model.
    /// Defines validation rules for the properties of a BookDto.
    /// </summary>
    public class BookDtoValidator : AbstractValidator<BookDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookDtoValidator"/> class.
        /// </summary>
        public BookDtoValidator()
        {

            RuleFor(dto => dto.Guid)
                .NotEmpty().WithMessage("Guid cannot be empty.")
                .NotEqual(Guid.Empty).WithMessage("Guid must be a valid GUID.");

            // Rules for Book-specific properties
            RuleFor(dto => dto.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot be longer than 200 characters.");

            RuleFor(dto => dto.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100).WithMessage("Author name cannot be longer than 100 characters.");

            RuleFor(dto => dto.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Length(10, 13).WithMessage("ISBN must be between 10 and 13 characters.") // Common ISBN-10 and ISBN-13
                .Matches(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$").WithMessage("Invalid ISBN format.");

            RuleFor(dto => dto.Description)
                .MaximumLength(2000).WithMessage("Description cannot be longer than 2000 characters.")
                .When(dto => !string.IsNullOrEmpty(dto.Description)); // Only validate length if description is provided

            RuleFor(dto => dto.PublicationYear)
                .InclusiveBetween(1000, DateTime.UtcNow.Year + 5)
                .WithMessage(dto => $"Please enter a valid publication year between 1000 and {DateTime.UtcNow.Year + 5}.") // Allow a bit into the future
                .When(dto => dto.PublicationYear.HasValue); // Only validate if publication year is provided

            RuleFor(dto => dto.Genre)
                .MaximumLength(50).WithMessage("Genre cannot be longer than 50 characters.")
                .When(dto => !string.IsNullOrEmpty(dto.Genre));

            RuleFor(dto => dto.CoverImageUrl)
                .MaximumLength(500).WithMessage("Cover image URL cannot be longer than 500 characters.")
                .Matches(@"^(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$").WithMessage("Please enter a valid URL for the cover image.")
                .When(dto => !string.IsNullOrEmpty(dto.CoverImageUrl));
        }
    }
}