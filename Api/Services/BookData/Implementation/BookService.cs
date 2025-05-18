using BookKeepAPI.Application.Dtos.BookData;
using BookKeepAPI.Application.Managers.BookData;
using BookKeepAPI.Application.Models.BookData;

namespace BookKeepAPI.Api.Services.BookData.Implementation;

/// <summary>
/// Implements the <see cref="IBookService"/> interface, providing a service layer
/// for book-related operations by orchestrating calls to the <see cref="IBookManager"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BookService"/> class.
/// </remarks>
/// <param name="bookManager">The book manager to be used for data operations.</param>
/// <exception cref="ArgumentNullException">Thrown if <paramref name="bookManager"/> is null.</exception>
public class BookService(IBookManager bookManager) : IBookService
{
    private readonly IBookManager _bookManager = bookManager ?? throw new ArgumentNullException(nameof(bookManager));

    /// <inheritdoc />
    public async Task<IBook?> GetBookByIdAsync(ulong id)
    {
        return await _bookManager.GetBookByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IBook>> GetAllBooksAsync()
    {
        return await _bookManager.GetAllBooksAsync();
    }

    /// <inheritdoc />
    public async Task<IBook> CreateBookAsync(BookDto bookDto)
    {
        return await _bookManager.CreateBookAsync(bookDto);
    }

    /// <inheritdoc />
    public async Task<IBook> UpdateBookAsync(ulong id, BookDto bookDto)
    {
        return await _bookManager.UpdateBookAsync(id, bookDto);
    }

    /// <inheritdoc />
    public async Task<IBook> DeleteBookAsync(ulong id)
    {
        return await _bookManager.DeleteBookAsync(id);
    }
}