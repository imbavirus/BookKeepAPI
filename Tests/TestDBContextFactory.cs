using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BookKeepAPI.Application.Data;

namespace BookKeepAPI.Tests;

/// <summary>
/// A factory for creating instances of <see cref="AppDbContext"/> configured for testing purposes.
/// It uses an in-memory SQLite database to ensure tests are isolated and fast.
/// Implements <see cref="IDisposable"/> to manage the lifetime of the SQLite connection.
/// </summary>
public class TestDbContextFactory : IDisposable
{
    private SqliteConnection? _connection;

    /// <summary>
    /// Creates and returns a new instance of <see cref="AppDbContext"/> connected to an in-memory SQLite database.
    /// Ensures that the database schema is created.
    /// </summary>
    /// <returns>A new <see cref="AppDbContext"/> instance ready for use in tests.</returns>
    public AppDbContext CreateContext()
    {
        // Using "DataSource=:memory:" creates an in-memory SQLite database.
        // It's crucial to keep the connection open for the lifetime of the DbContext
        // when using in-memory SQLite, otherwise the database is deleted when the connection closes.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    /// <summary>
    /// Disposes of the underlying SQLite connection, effectively closing and deleting the in-memory database.
    /// It also suppresses garbage collector finalization for this object, as cleanup is handled by this method.
    /// </summary>
    public void Dispose()
    {
        _connection?.Dispose(); // Close and dispose the connection
        GC.SuppressFinalize(this);
    }
}