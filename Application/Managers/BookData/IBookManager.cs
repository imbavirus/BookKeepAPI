using BookKeepAPI.Application.Dtos.BookData;
using BookKeepAPI.Application.Models.BookData; // Updated to use Book models

namespace BookKeepAPI.Application.Managers.BookData; // Updated namespace

/// <summary>
/// Defines the contract for managing <see cref="Book"/> entities.
/// </summary>
public interface IBookManager
{
    /// <summary>
    /// Retrieves a book by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="IBook"/> if found; otherwise, null.
    /// </returns>
    Task<IBook?> GetBookByIdAsync(ulong id);

    /// <summary>
    /// Retrieves all books asynchronously.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumerable collection of <see cref="IBook"/>.
    /// </returns>
    Task<IEnumerable<IBook>> GetAllBooksAsync();

    /// <summary>
    /// Updates an existing book asynchronously.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="bookUpdate">The <see cref="BookDto"/> object with updated information. Note: Using concrete type for input.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="IBook"/>.</returns>
    Task<IBook> UpdateBookAsync(ulong id, BookDto bookUpdate);

    /// <summary>
    /// Creates a new book asynchronously.
    /// </summary>
    /// <param name="newBook">The <see cref="BookDto"/> object to create. Note: Using concrete type for input.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="IBook"/>.</returns>
    Task<IBook> CreateBookAsync(BookDto newBook);
    /// <summary>
    /// Deletes a book by marking it as inactive (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the book to delete.</param>
    /// <returns>The <see cref="IBook"/> that was marked as inactive.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the book with the specified Id does not exist.</exception>
    Task<IBook> DeleteBookAsync(ulong id);
}