using Microsoft.EntityFrameworkCore;
using BookKeepAPI.Application.Models;
using BookKeepAPI.Application.Models.BookData;

namespace BookKeepAPI.Application.Data;

/// <summary>
/// Represents the application's database context, responsible for interacting with the underlying database.
/// It includes DbSet properties for database entities and overrides SaveChanges methods to handle audit properties.
/// </summary>
/// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the DbSet for <see cref="Book"/> entities, allowing querying and saving of book data.
    /// </summary>
    public DbSet<Book> Books { get; set; }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// Automatically sets audit properties (CreatedOn, UpdatedOn) before saving.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Automatically sets audit properties (CreatedOn, UpdatedOn) before saving.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        SetAuditProperties();
        return base.SaveChanges();
    }

    /// <summary>
    /// Sets the audit properties (CreatedOn and UpdatedOn) for entities that implement <see cref="IBaseModel"/>
    /// and are being added or modified.
    /// </summary>
    private void SetAuditProperties()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IBaseModel && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var dateNow = DateTime.UtcNow;
            var baseModel = (IBaseModel)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                baseModel.CreatedOn = dateNow;
                baseModel.UpdatedOn = dateNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                baseModel.UpdatedOn = dateNow;
                entityEntry.Property(nameof(IBaseModel.CreatedOn)).IsModified = false;
            }
        }
    }
}