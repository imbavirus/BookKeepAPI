using System.ComponentModel.DataAnnotations;

namespace BookKeepAPI.Application.Models;

/// <summary>
/// Defines the basic properties that all models in the application should have.
/// This includes identifiers and audit timestamps.
/// </summary>
public interface IBaseModel
{
    /// <summary>
    /// Gets or sets the unique primary key for the entity.
    /// Typically an auto-incrementing integer.
    /// </summary>
    ulong Id { get; set; }
    /// <summary>
    /// Gets or sets the globally unique identifier (GUID) for the entity.
    /// Provides a universally unique ID that can be generated client-side or server-side.
    /// </summary>
    Guid Guid { get; set; }
    /// <summary>
    /// Gets or sets the date and time when the entity was created, in UTC.
    /// </summary>
    DateTime CreatedOn { get; set; }
    /// <summary>
    /// Gets or sets the date and time when the entity was last updated, in UTC.
    /// </summary>
    DateTime UpdatedOn { get; set; }

    /* These would additionally be added if I were to add user authentication */
    // /// <summary>
    // /// Gets or sets the ID of the user who created this entity. Nullable if the creator is anonymous or not tracked.
    // /// </summary>
    // ulong? CreatedByUserProfileId { get; set; }
    // /// <summary>
    // /// Gets or sets the navigation property to the UserProfile of the user who created this entity.
    // /// </summary>
    // UserProfile? CreatedByUserProfile { get; set; }
    // /// <summary>
    // /// Gets or sets the ID of the user who last updated this entity. Nullable if the updater is anonymous or not tracked.
    // /// </summary>
    // ulong? UpdatedByUserProfileId { get; set; }
    // /// <summary>
    // /// Gets or sets the navigation property to the UserProfile of the user who last updated this entity.
    // /// </summary>
    // UserProfile? UpdatedByUserProfile { get; set; }
}

/// <summary>
/// Provides a base implementation for models in the application,
/// including common properties like ID, GUID, and audit timestamps.
/// </summary>
public abstract class BaseModel : IBaseModel
{
    /// <summary>
    /// Gets or sets the unique primary key for the entity.
    /// This property is typically used as the primary key in the database.
    /// </summary>
    [Key]
    public ulong Id { get; set; }

    /// <summary>
    /// Gets or sets the globally unique identifier (GUID) for the entity.
    /// This ensures a unique identifier across different databases or systems.
    /// </summary>
    public Guid Guid { get; set; } 

    /// <summary>
    /// Gets or sets the date and time when the entity was created, in UTC.
    /// This is automatically set in the constructor.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated, in UTC.
    /// This is automatically set in the constructor and should be updated on subsequent modifications.
    /// </summary>
    public DateTime UpdatedOn { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseModel"/> class.
    /// Sets the <see cref="CreatedOn"/> and <see cref="UpdatedOn"/> timestamps to the current UTC time.
    /// </summary>
    protected BaseModel()
    {
        CreatedOn = DateTime.UtcNow;
        UpdatedOn = DateTime.UtcNow; // On creation, UpdatedOn is the same as CreatedOn
    }

    /* These would additionally be added if I were to add user authentication */
    // /// <summary>
    // /// Gets or sets the foreign key for the user who created this entity.
    // /// This would be used if user tracking for creation is implemented.
    // /// </summary>
    // public ulong? CreatedByUserProfileId { get; set; }

    // /// <summary>
    // /// Gets or sets the navigation property to the UserProfile of the user who created this entity.
    // /// This would be used if user tracking for creation is implemented.
    // /// </summary>
    // [ForeignKey("CreatedByUserProfileId")]
    // public virtual UserProfile? CreatedByUserProfile { get; set; }

    // /// <summary>
    // /// Gets or sets the foreign key for the user who last updated this entity.
    // /// This would be used if user tracking for updates is implemented.
    // /// </summary>
    // public ulong? UpdatedByUserProfileId { get; set; }
    // /// <summary>
    // /// Gets or sets the navigation property to the UserProfile of the user who last updated this entity.
    // /// This would be used if user tracking for updates is implemented.
    // /// </summary>
    // [ForeignKey("UpdatedByUserProfileId")]
    // public virtual UserProfile? UpdatedByUserProfile { get; set; }
}
