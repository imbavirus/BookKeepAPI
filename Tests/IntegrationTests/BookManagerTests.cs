using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using BookKeepAPI.Application.Data;
using BookKeepAPI.Application.Managers.BookData.Implementation;
using BookKeepAPI.Application.Models.BookData;
using BookKeepAPI.Application.Dtos.BookData;
using Xunit;
using Moq;
using BookKeepAPI.Application.Managers.External.OpenLibrary;

namespace BookKeepAPI.Tests.IntegrationTests;

/// <summary>
/// Integration tests for the <see cref="BookManager"/> class.
/// These tests verify the interaction of the BookManager with the database
/// for CRUD operations and business logic related to Book entities.
/// </summary>
public class BookManagerTests : IDisposable
{
    private readonly TestDbContextFactory _dbContextFactory;
    private readonly Mock<IOpenLibraryManager> _mockOpenLibraryManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookManagerTests"/> class.
    /// Sets up the <see cref="TestDbContextFactory"/> for creating in-memory database contexts.
    /// </summary>
    public BookManagerTests()
    {
        _dbContextFactory = new TestDbContextFactory();
        _mockOpenLibraryManager = new Mock<IOpenLibraryManager>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="BookManager"/> with the provided <see cref="AppDbContext"/>.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="OpenLibraryManager">The Open Library service mock.</param>
    /// <returns>A new <see cref="BookManager"/> instance.</returns>
    private static BookManager CreateManager(AppDbContext context, IOpenLibraryManager OpenLibraryManager)
    {
        return new BookManager(context, OpenLibraryManager);
    }
    /// <summary>
    /// Creates a sample <see cref="Book"/> entity, adds it to the context, and saves changes.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="title">The title of the book.</param>
    /// <param name="isbn">The ISBN of the book.</param>
    /// <param name="isActive">A flag indicating if the book is active.</param>
    /// <param name="guid">The optional GUID for the book; if null, a new GUID is generated.</param>
    /// <returns>The created and saved <see cref="Book"/> entity.</returns>
    private static Book CreateAndAddSampleBook(AppDbContext context, string title = "Sample Book", string isbn = "1234567890", bool isActive = true, Guid? guid = null)
    {
        var book = new Book(title, "Sample Author", isbn, "Sample Description", 2020, "Fiction", "http://example.com/cover.jpg")
        {
            Guid = guid ?? Guid.NewGuid(),
            IsActive = isActive
        };
        context.Books.Add(book);
        context.SaveChanges(); // Save to get Id assigned
        return book;
    }

    /// <summary>
    /// Creates a sample <see cref="BookDto"/> object.
    /// </summary>
    /// <param name="title">The title for the DTO.</param>
    /// <param name="author">The author for the DTO.</param>
    /// <param name="isbn">The ISBN for the DTO.</param>
    /// <param name="guid">The optional GUID for the DTO; if null, a new GUID is generated.</param>
    /// <returns>A new <see cref="BookDto"/> instance.</returns>
    private static BookDto CreateSampleBookDto(string title = "DTO Book", string author = "DTO Author", string isbn = "0987654321", Guid? guid = null)
    {
        return new BookDto
        {
            Guid = guid ?? Guid.NewGuid(),
            Title = title,
            Author = author,
            ISBN = isbn,
            Description = "DTO Description",
            PublicationYear = 2021,
            Genre = "Science Fiction",
            CoverImageUrl = "http://example.com/dto_cover.jpg"
        };
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.GetBookByIdAsync"/> returns a book when it exists and is active.
    /// </summary>
    [Fact]
    public async Task GetBookByIdAsync_ShouldReturnBook_WhenBookExistsAndIsActive()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var seededBook = CreateAndAddSampleBook(context, isActive: true);

        // Act
        var actualBook = await manager.GetBookByIdAsync(seededBook.Id);

        // Assert
        actualBook.Should().NotBeNull();
        actualBook.Should().BeEquivalentTo(seededBook, options => options
            .Excluding(b => b.CreatedOn) // EF Core might set these with slight precision differences
            .Excluding(b => b.UpdatedOn));
        actualBook!.CreatedOn.Should().BeCloseTo(seededBook.CreatedOn, TimeSpan.FromSeconds(1));
        actualBook!.UpdatedOn.Should().BeCloseTo(seededBook.UpdatedOn, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.GetBookByIdAsync"/> returns null when a book exists but is not active.
    /// </summary>
    [Fact]
    public async Task GetBookByIdAsync_ShouldReturnNull_WhenBookExistsButIsNotActive()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var seededBook = CreateAndAddSampleBook(context, isActive: false);

        // Act
        var result = await manager.GetBookByIdAsync(seededBook.Id);

        // Assert
        result.Should().BeNull(because: "GetBookByIdAsync should only return active books");
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.GetBookByIdAsync"/> returns null when a book does not exist.
    /// </summary>
    [Fact]
    public async Task GetBookByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var nonExistentId = 999UL;

        // Act
        var result = await manager.GetBookByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.GetAllBooksAsync"/> returns all active books when books exist.
    /// </summary>
    [Fact]
    public async Task GetAllBooksAsync_ShouldReturnAllActiveBooks_WhenBooksExist()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var activeBook1 = CreateAndAddSampleBook(context, title: "Active Book 1", isbn: "111", isActive: true);
        var activeBook2 = CreateAndAddSampleBook(context, title: "Active Book 2", isbn: "222", isActive: true);
        CreateAndAddSampleBook(context, title: "Inactive Book", isbn: "333", isActive: false); // Inactive book

        var expectedActiveBooks = new List<Book> { activeBook1, activeBook2 }.OrderBy(b => b.Id).ToList();

        // Act
        var actualBooks = (await manager.GetAllBooksAsync()).OrderBy(b => b.Id).ToList();

        // Assert
        actualBooks.Should().NotBeNullOrEmpty();
        actualBooks.Should().HaveSameCount(expectedActiveBooks);
        actualBooks.Should().BeEquivalentTo(expectedActiveBooks, options => options
            .Excluding(b => b.CreatedOn)
            .Excluding(b => b.UpdatedOn)
            .WithStrictOrdering());

        for(int i = 0; i < expectedActiveBooks.Count; i++)
        {
            actualBooks[i].CreatedOn.Should().BeCloseTo(expectedActiveBooks[i].CreatedOn, TimeSpan.FromSeconds(1));
            actualBooks[i].UpdatedOn.Should().BeCloseTo(expectedActiveBooks[i].UpdatedOn, TimeSpan.FromSeconds(1));
        }
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.GetAllBooksAsync"/> returns an empty list when no active books exist.
    /// </summary>
    [Fact]
    public async Task GetAllBooksAsync_ShouldReturnEmptyList_WhenNoActiveBooksExist()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        CreateAndAddSampleBook(context, title: "Inactive Book Only", isbn: "777", isActive: false);
        await context.SaveChangesAsync();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);

        // Act
        var result = await manager.GetAllBooksAsync();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.CreateBookAsync"/> creates and returns a book when the provided data is valid.
    /// </summary>
    [Fact]
    public async Task CreateBookAsync_ShouldCreateAndReturnBook_WhenDataIsValid()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var bookDto = CreateSampleBookDto(guid: Guid.NewGuid());
        var utcNowBeforeCreate = DateTime.UtcNow;

        // Act
        var createdBook = await manager.CreateBookAsync(bookDto);
        var savedBook = await context.Books.FindAsync(createdBook.Id);

        // Assert
        createdBook.Should().NotBeNull();
        createdBook.Title.Should().Be(bookDto.Title);
        createdBook.Author.Should().Be(bookDto.Author);
        createdBook.ISBN.Should().Be(bookDto.ISBN);
        createdBook.Guid.Should().Be(bookDto.Guid);
        createdBook.IsActive.Should().BeTrue();

        savedBook.Should().NotBeNull();
        savedBook!.Title.Should().Be(bookDto.Title);
        savedBook.Guid.Should().Be(bookDto.Guid);
        savedBook.IsActive.Should().BeTrue();
        savedBook.CreatedOn.Should().BeCloseTo(utcNowBeforeCreate, TimeSpan.FromSeconds(2));
        savedBook.UpdatedOn.Should().Be(savedBook.CreatedOn);
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.CreateBookAsync"/> generates a new GUID if the DTO's GUID is empty.
    /// </summary>
    [Fact]
    public async Task CreateBookAsync_ShouldGenerateNewGuid_WhenDtoGuidIsEmpty()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var bookDto = CreateSampleBookDto(guid: Guid.Empty); // Manager should generate a new Guid

        // Act
        var createdBook = await manager.CreateBookAsync(bookDto);
        var savedBook = await context.Books.FindAsync(createdBook.Id);

        // Assert
        createdBook.Guid.Should().NotBeEmpty();
        savedBook!.Guid.Should().NotBeEmpty();
        createdBook.Guid.Should().Be(savedBook.Guid);
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.CreateBookAsync"/> throws an <see cref="InvalidOperationException"/> when a book with the same GUID already exists and is active.
    /// </summary>
    [Fact]
    public async Task CreateBookAsync_ShouldThrowInvalidOperationException_WhenBookWithSameGuidExistsAndIsActive()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var existingGuid = Guid.NewGuid();
        CreateAndAddSampleBook(context, guid: existingGuid, isActive: true);
        var bookDto = CreateSampleBookDto(guid: existingGuid);

        // Act
        Func<Task> act = async () => await manager.CreateBookAsync(bookDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"A Book with Guid '{existingGuid}' already exists.");
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.CreateBookAsync"/> succeeds when a book with the same GUID exists but is inactive.
    /// </summary>
    [Fact]
    public async Task CreateBookAsync_ShouldSucceed_WhenBookWithSameGuidExistsButIsNotActive()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var existingGuid = Guid.NewGuid();
        CreateAndAddSampleBook(context, guid: existingGuid, isActive: false); // Existing book is inactive
        var bookDto = CreateSampleBookDto(guid: existingGuid, title: "New Book Same Guid Inactive");

        // Act
        var createdBook = await manager.CreateBookAsync(bookDto);

        // Assert
        createdBook.Should().NotBeNull();
        createdBook.Guid.Should().Be(existingGuid);
        createdBook.IsActive.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.CreateBookAsync"/> throws an <see cref="InvalidOperationException"/> when a book with the same ISBN already exists and is active.
    /// </summary>
    [Fact]
    public async Task CreateBookAsync_ShouldThrowInvalidOperationException_WhenBookWithSameISBNExistsAndIsActive()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var existingISBN = "111222333X";
        CreateAndAddSampleBook(context, isbn: existingISBN, isActive: true);
        var bookDto = CreateSampleBookDto(isbn: existingISBN);

        // Act
        Func<Task> act = async () => await manager.CreateBookAsync(bookDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"A Book with ISBN '{existingISBN}' already exists.");
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.CreateBookAsync"/> succeeds when a book with the same ISBN exists but is inactive.
    /// </summary>
    [Fact]
    public async Task CreateBookAsync_ShouldSucceed_WhenISBNExistsOnInactiveBook()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var existingISBN = "9876543210";
        CreateAndAddSampleBook(context, title: "Inactive Book Old ISBN", isbn: existingISBN, isActive: false); // Existing book is inactive

        var bookDto = CreateSampleBookDto(title: "New Book Same ISBN As Inactive", isbn: existingISBN, guid: Guid.NewGuid());

        // Act
        var createdBook = await manager.CreateBookAsync(bookDto);

        // Assert
        createdBook.Should().NotBeNull();
        createdBook.ISBN.Should().Be(existingISBN);
        createdBook.IsActive.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.UpdateBookAsync"/> updates and returns a book when the provided data is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBookAsync_ShouldUpdateAndReturnBook_WhenDataIsValid()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var bookToUpdate = CreateAndAddSampleBook(context, title: "Original Title", isbn: "original-isbn");

        // Detach to simulate fetching and then updating
        var originalCreatedOn = bookToUpdate.CreatedOn;
        var originalGuid = bookToUpdate.Guid;
        context.Entry(bookToUpdate).State = EntityState.Detached;

        var bookUpdateDto = CreateSampleBookDto(title: "Updated Title", author: "Updated Author", isbn: "updated-isbn", guid: Guid.NewGuid()); // Guid in DTO should be ignored by update

        var utcNowBeforeUpdate = DateTime.UtcNow;
        await Task.Delay(50); // Ensure UpdatedOn will be different

        // Act
        var result = await manager.UpdateBookAsync(bookToUpdate.Id, bookUpdateDto);
        var savedBook = await context.Books.FindAsync(bookToUpdate.Id);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(bookUpdateDto.Title);
        result.Author.Should().Be(bookUpdateDto.Author);
        result.ISBN.Should().Be(bookUpdateDto.ISBN);
        result.Id.Should().Be(bookToUpdate.Id);
        result.Guid.Should().Be(originalGuid); // Guid should not change on update
        result.IsActive.Should().Be(bookToUpdate.IsActive); // IsActive should not change on update

        savedBook.Should().NotBeNull();
        savedBook!.Title.Should().Be(bookUpdateDto.Title);
        savedBook.CreatedOn.Should().Be(originalCreatedOn); // CreatedOn should not change
        savedBook.UpdatedOn.Should().BeCloseTo(utcNowBeforeUpdate, TimeSpan.FromSeconds(2));
        savedBook.UpdatedOn.Should().BeAfter(bookToUpdate.UpdatedOn);
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.UpdateBookAsync"/> throws a <see cref="KeyNotFoundException"/> when the book to update does not exist.
    /// </summary>
    [Fact]
    public async Task UpdateBookAsync_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var nonExistentId = 999UL;
        var bookUpdateDto = CreateSampleBookDto();

        // Act
        Func<Task> act = async () => await manager.UpdateBookAsync(nonExistentId, bookUpdateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Book with Id '{nonExistentId}' does not exist.");
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.UpdateBookAsync"/> throws an <see cref="InvalidOperationException"/> when attempting to update a book's ISBN to one that already exists on another active book.
    /// </summary>
    [Fact]
    public async Task UpdateBookAsync_ShouldThrowInvalidOperationException_WhenUpdatingToExistingISBNOfAnotherActiveBook()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var bookToUpdate = CreateAndAddSampleBook(context, title: "Book To Update", isbn: "isbn-to-update", isActive: true);
        var otherBook = CreateAndAddSampleBook(context, title: "Other Book", isbn: "existing-isbn", isActive: true);

        var bookUpdateDto = CreateSampleBookDto(isbn: otherBook.ISBN); // Attempt to set ISBN to otherBook's ISBN

        // Act
        Func<Task> act = async () => await manager.UpdateBookAsync(bookToUpdate.Id, bookUpdateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Another book with the ISBN '{otherBook.ISBN}' already exists.");
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.DeleteBookAsync"/> marks a book as inactive (soft delete) and returns the updated book.
    /// </summary>
    [Fact]
    public async Task DeleteBookAsync_ShouldMarkBookAsInactiveAndReturnIt()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var bookToDelete = CreateAndAddSampleBook(context, isActive: true);
        var originalUpdatedOn = bookToDelete.UpdatedOn;
        await Task.Delay(50); // Ensure UpdatedOn will be different
        var utcNowBeforeDelete = DateTime.UtcNow;

        // Act
        var result = await manager.DeleteBookAsync(bookToDelete.Id);
        var savedBook = await context.Books.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == bookToDelete.Id); // Use IgnoreQueryFilters if you have global query filters for IsActive

        // Assert
        result.Should().NotBeNull();
        result.IsActive.Should().BeFalse();

        savedBook.Should().NotBeNull();
        savedBook!.IsActive.Should().BeFalse();
        savedBook.UpdatedOn.Should().BeCloseTo(utcNowBeforeDelete, TimeSpan.FromSeconds(2));
        savedBook.UpdatedOn.Should().BeAfter(originalUpdatedOn);
    }

    /// <summary>
    /// Verifies that <see cref="BookManager.DeleteBookAsync"/> throws a <see cref="KeyNotFoundException"/> when the book to delete does not exist.
    /// </summary>
    [Fact]
    public async Task DeleteBookAsync_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context, _mockOpenLibraryManager.Object);
        var nonExistentId = 999UL;

        // Act
        Func<Task> act = async () => await manager.DeleteBookAsync(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Book with Id '{nonExistentId}' not found.");
    }

    /// <summary>
    /// Disposes of the <see cref="TestDbContextFactory"/> instance.
    /// </summary>
    public void Dispose()
    {
        _dbContextFactory.Dispose();
        GC.SuppressFinalize(this);
    }
}
