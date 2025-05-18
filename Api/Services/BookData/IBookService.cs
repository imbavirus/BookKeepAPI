using BookKeepAPI.Application.Dtos.BookData;
using BookKeepAPI.Application.Models.BookData;

namespace BookKeepAPI.Api.Services.BookData;

/// <summary>
/// Defines the contract for a service that manages book-related operations.
/// This service layer typically orchestrates calls to managers or repositories
/// and can include additional business logic, validation, or mapping.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Retrieves a book by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="IBook"/> if found and active; otherwise, null.
    /// </returns>
    Task<IBook?> GetBookByIdAsync(ulong id);

    /// <summary>
    /// Retrieves all active books asynchronously.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumerable collection of <see cref="IBook"/>.
    /// </returns>
    Task<IEnumerable<IBook>> GetAllBooksAsync();

    /// <summary>
    /// Creates a new book asynchronously using the provided DTO.
    /// </summary>
    /// <param name="bookDto">The data transfer object containing the information for the new book.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="IBook"/>.</returns>
    Task<IBook> CreateBookAsync(BookDto bookDto);

    /// <summary>
    /// Updates an existing book asynchronously using the provided DTO.
    /// </summary>
    /// <param name="id">The unique identifier of the book to update.</param>
    /// <param name="bookDto">The data transfer object containing the updated information for the book.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="IBook"/>.</returns>
    Task<IBook> UpdateBookAsync(ulong id, BookDto bookDto);

    /// <summary>
    /// Deletes a book by its unique identifier (soft delete, marking as inactive).
    /// </summary>
    /// <param name="id">The unique identifier of the book to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="IBook"/> that was marked as inactive.</returns>
    Task<IBook> DeleteBookAsync(ulong id);
}
