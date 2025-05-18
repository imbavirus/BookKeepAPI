namespace BookKeepAPI.Application.Models.Exception;

/// <summary>
/// Represents a standardized error response returned by the API.
/// </summary>
/// <param name="message">A human-readable message describing the error.</param>
/// <param name="type">A string indicating the type or category of the error.</param>
/// <param name="payload">Optional additional data related to the error.</param>
public class ErrorResponse(string message, string type, object? payload = null)
{
    /// <summary>
    /// Gets or sets the human-readable message describing the error.
    /// </summary>
    public string Message { get; set; } = message;

    /// <summary>
    /// Gets a value indicating whether this response represents an error. Always true for ErrorResponse.
    /// </summary>
    public bool HasError = true;

    /// <summary>
    /// Gets or sets a string indicating the type or category of the error (e.g., "Validation Error", "Not Found").
    /// </summary>
    public string Type { get; set; } = type;

    /// <summary>
    /// Gets or sets an optional payload containing additional details or data related to the error.
    /// </summary>
    public object? Payload { get; set; } = payload;
}
