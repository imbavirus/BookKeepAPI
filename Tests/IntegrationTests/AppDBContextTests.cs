using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using BookKeepAPI.Application.Models.BookData;
using Xunit;

namespace BookKeepAPI.Tests.IntegrationTests;

public class AppDbContextTests
{
    [Fact]
    public async Task SaveChanges_ShouldSetAuditPropertiesCorrectly()
    {
        // Arrange
        using var dbContextFactory = new TestDbContextFactory();
        using var dbContext = dbContextFactory.CreateContext();

        var newBook = new Book(
            title: "Audit Test Book",
            author: "Audit Author",
            isbn: "978-0000000001",
            description: "Testing audit properties for a new book.")
        {
            Guid = Guid.NewGuid() // Assign a new Guid for this test entity
        };

        // Act (Add)
        var utcNowBeforeAdd = DateTime.UtcNow;
        dbContext.Books.Add(newBook);
        await dbContext.SaveChangesAsync();

        // Assert (Add)
        // Detach and re-fetch to ensure we're getting data from the DB, not just the tracked entity.
        dbContext.Entry(newBook).State = EntityState.Detached;
        var addedProfile = await dbContext.Books.FindAsync(newBook.Id);

        addedProfile.Should().NotBeNull(because: "the book should have been saved to the database");
        addedProfile.CreatedOn.Should().BeCloseTo(utcNowBeforeAdd, TimeSpan.FromSeconds(2),
            because: "CreatedOn should be set to the current UTC time when an entity is added");
        addedProfile!.UpdatedOn.Should().Be(addedProfile.CreatedOn,
            because: "UpdatedOn should be the same as CreatedOn for a newly added entity");

        // Arrange (Modify)
        var originalCreatedOn = addedProfile.CreatedOn;
        var originalUpdatedOn = addedProfile.UpdatedOn;

        // Ensure a small delay so UtcNow is different enough to be noticeable
        await Task.Delay(50);

        addedProfile.Title = "Updated Audit Test Book Title";
        addedProfile.Description = "Description has been updated.";
        var utcNowBeforeModify = DateTime.UtcNow;

        // Act (Modify)
        // Re-attach the modified entity for saving
        dbContext.Books.Update(addedProfile);
        await dbContext.SaveChangesAsync();

        // Assert (Modify)
        dbContext.Entry(addedProfile).State = EntityState.Detached;
        var modifiedProfile = await dbContext.Books.FindAsync(addedProfile.Id);

        modifiedProfile.Should().NotBeNull();
        modifiedProfile!.Title.Should().Be("Updated Audit Test Book Title");
        modifiedProfile.Description.Should().Be("Description has been updated.");
        modifiedProfile.CreatedOn.Should().Be(originalCreatedOn,
            because: "CreatedOn should not change when an entity is updated");
        modifiedProfile.UpdatedOn.Should().BeCloseTo(utcNowBeforeModify, TimeSpan.FromSeconds(2),
            because: "UpdatedOn should be set to the current UTC time when an entity is modified");
        modifiedProfile.UpdatedOn.Should().BeAfter(originalUpdatedOn,
            because: "UpdatedOn should be later than its previous value after modification");
    }
}
