using System.Text.Json;

namespace BookKeepAPI.Application.Helpers;

/// <summary>
/// Provides static helper methods for common utility functions.
/// </summary>
public static class Helper
{
    /// <summary>
    /// Gets the default JSON serializer options used by helper methods.
    /// These options are configured for human-readable output (indented)
    /// and to handle potential circular references in object graphs by preserving them.
    /// This instance is cached for performance.
    /// </summary>
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        WriteIndented = true, // Makes the JSON output human-readable.
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    };

    /// <summary>
    /// Serializes an object to a JSON string with indentation and reference handling, then prints it to the console.
    /// </summary>
    /// <param name="obj">The object to print.</param>
    public static void PrintObject(object obj)
    {
        string printValue = JsonSerializer.Serialize(obj, DefaultJsonSerializerOptions);
        Console.WriteLine(printValue);
    }

    /// <summary>
    /// Serializes an object to a JSON string with indentation and reference handling.
    /// </summary>
    /// <param name="obj">The object to stringify.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string StringifyObject(object obj)
    {
        string printValue = JsonSerializer.Serialize(obj, DefaultJsonSerializerOptions);
        return printValue;
    }    
}
