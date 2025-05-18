using Microsoft.AspNetCore.Mvc;
using BookKeepAPI.Api.Services.BookData;
using BookKeepAPI.Application.Models.BookData;
using BookKeepAPI.Application.Dtos.BookData;

namespace BookKeepAPI.Api.Controllers.BookData;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookService bookService, ILogger<BooksController> logger) : ControllerBase
{
    private readonly IBookService _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
    private readonly ILogger<BooksController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets a specific book by its ID.
    /// </summary>
    /// <param name="id">The ID of the book to retrieve.</param>
    /// <returns>The book if found; otherwise, NotFound.</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(IBook), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IBook>> GetBookById(ulong id)
    {
        _logger.LogInformation("Attempting to get book with ID: {BookId}", id);
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            _logger.LogWarning("Book with ID: {BookId} not found.", id);
            return NotFound();
        }
        _logger.LogInformation("Successfully retrieved book with ID: {BookId}", id);
        return Ok(book);
    }

    /// <summary>
    /// Gets all books.
    /// </summary>
    /// <returns>A list of all books.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IBook>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<IBook>>> GetAllBooks()
    {
        _logger.LogInformation("Attempting to get all books");
        var books = await _bookService.GetAllBooksAsync();
        _logger.LogInformation("Successfully retrieved {Count} books", books.Count());
        return Ok(books);
    }

    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="bookDto">The book data to create.</param>
    /// <returns>The created book.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(IBook), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IBook>> CreateBook([FromBody] BookDto bookDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("CreateBook request failed due to invalid model state.");
            return BadRequest(ModelState);
        }
        _logger.LogInformation("Attempting to create a new book with title: {BookTitle}", bookDto.Title);
        var createdBook = await _bookService.CreateBookAsync(bookDto);
        _logger.LogInformation("Successfully created book with ID: {BookId}", createdBook.Id);
        return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
    }

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <param name="bookDto">The book data to update.</param>
    /// <returns>NoContent if successful; otherwise, BadRequest or NotFound.</returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(IBook), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IBook>> UpdateBook(ulong id, [FromBody] BookDto bookDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("UpdateBook request for ID {BookId} failed due to invalid model state.", id);
            return BadRequest(ModelState);
        }
        _logger.LogInformation("Attempting to update book with ID: {BookId}", id);
        var updatedBook = await _bookService.UpdateBookAsync(id, bookDto);
        _logger.LogInformation("Successfully updated book with ID: {BookId}", updatedBook.Id);
        return Ok(updatedBook);
    }

    /// <summary>
    /// Deletes a book by its ID (soft delete).
    /// </summary>
    /// <param name="id">The ID of the book to delete.</param>
    /// <returns>NoContent if successful; otherwise, NotFound.</returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(ulong id)
    {
        _logger.LogInformation("Attempting to delete book with ID: {BookId}", id);
        await _bookService.DeleteBookAsync(id); // IBookService.DeleteBookAsync returns IBook, but for a DELETE op, 204 is common.
        _logger.LogInformation("Successfully marked book with ID: {BookId} as inactive.", id);
        return NoContent();
    }
}