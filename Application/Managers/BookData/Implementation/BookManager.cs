using Microsoft.EntityFrameworkCore;
using BookKeepAPI.Application.Data;
using BookKeepAPI.Application.Models.BookData;
using BookKeepAPI.Application.Dtos.BookData;

namespace BookKeepAPI.Application.Managers.BookData.Implementation;

/// <summary>
/// Manages CRUD operations for <see cref="Book"/> entities.
/// </summary>
public class BookManager(AppDbContext context) : IBookManager
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>The <see cref="IBook"/> if found; otherwise, null.</returns>
    public async Task<IBook?> GetBookByIdAsync(ulong id)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
    }

    /// <summary>
    /// Retrieves all active books.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="IBook"/>.</returns>
    public async Task<IEnumerable<IBook>> GetAllBooksAsync()
    {
        return await _context.Books.Where(x => x.IsActive).ToListAsync();
    }

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="bookUpdate">The book object with updated information.</param>
    /// <returns>The updated <see cref="IBook"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the book with the specified Id does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if another book with the same ISBN already exists.</exception>
    public async Task<IBook> UpdateBookAsync(ulong id, BookDto bookUpdate)
    {
        var existingBook = await _context.Books.FindAsync(id) ??
            throw new KeyNotFoundException($"Book with Id '{id}' does not exist.");

        // Check if another book with the new ISBN already exists (excluding the current book)
        if (!string.IsNullOrWhiteSpace(bookUpdate.ISBN))
        {
            var bookWithSameIsbn = await _context.Books
                .FirstOrDefaultAsync(b => b.ISBN == bookUpdate.ISBN && b.Id != id && b.IsActive);

            if (bookWithSameIsbn != null)
                throw new InvalidOperationException($"Another book with the ISBN '{bookUpdate.ISBN}' already exists.");
        }

        // Update properties
        existingBook.Title = bookUpdate.Title;
        existingBook.Author = bookUpdate.Author;
        existingBook.ISBN = bookUpdate.ISBN;
        existingBook.Description = bookUpdate.Description;
        existingBook.PublicationYear = bookUpdate.PublicationYear;
        existingBook.Genre = bookUpdate.Genre;
        existingBook.CoverImageUrl = bookUpdate.CoverImageUrl;
        // Guid should typically not be updated. CreatedOn is handled by AppDbContext.
        // UpdatedOn will be handled by AppDbContext.
        // IsActive is only set using delete and create

        _context.Books.Update(existingBook);
        await _context.SaveChangesAsync();

        return existingBook;
    }

    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="newBook">The book object to create.</param>
    /// <returns>The created <see cref="IBook"/> entity.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a book with the same Guid or ISBN already exists.</exception>
    public async Task<IBook> CreateBookAsync(BookDto newBook)
    {
        Guid entityGuid = newBook.Guid;

        // If Guid is not provided in the DTO (e.g., if validator didn't run or allows empty), generate one.
        // However, BookDtoValidator makes Guid mandatory and non-empty.
        // This block handles it defensively or if BookDto is used without prior validation in some contexts.
        if (entityGuid == Guid.Empty)
        {
            entityGuid = Guid.NewGuid();
        }
        else
        {
            // Guid was provided, check for uniqueness.
            var existingBookByGuid = await _context.Books.FirstOrDefaultAsync(b => b.Guid == entityGuid && b.IsActive);
            if (existingBookByGuid != null)
            {
                throw new InvalidOperationException($"A Book with Guid '{entityGuid}' already exists.");
            }
        }

        // Check if a book with the same ISBN already exists
        if (!string.IsNullOrWhiteSpace(newBook.ISBN))
        {
            var existingBookByIsbn = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == newBook.ISBN && b.IsActive);
            if (existingBookByIsbn != null)
                throw new InvalidOperationException($"A Book with ISBN '{newBook.ISBN}' already exists.");
        }

        var bookEntity = new Book(
            title: newBook.Title,
            author: newBook.Author,
            isbn: newBook.ISBN,
            description: newBook.Description,
            publicationYear: newBook.PublicationYear,
            genre: newBook.Genre,
            coverImageUrl: newBook.CoverImageUrl
        )
        {
            Guid = entityGuid, // Ensure the validated or generated Guid is set.
            IsActive = true   // New books are active by default.
        };
        // Id, CreatedOn, UpdatedOn are handled by BaseModel constructor or AppDbContext.

        _context.Books.Add(bookEntity);
        await _context.SaveChangesAsync();

        return bookEntity;
    }

    /// <summary>
    /// Deletes a book by marking it as inactive (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the book to delete.</param>
    /// <returns>The <see cref="IBook"/> that was marked as inactive.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the book with the specified Id does not exist.</exception>
    public async Task<IBook> DeleteBookAsync(ulong id)
    {
        var bookToDelete = await _context.Books.FindAsync(id) ?? throw new KeyNotFoundException($"Book with Id '{id}' not found.");
        bookToDelete.IsActive = false;

        await _context.SaveChangesAsync();
        return bookToDelete;
    }
}
