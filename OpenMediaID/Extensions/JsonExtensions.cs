using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenMediaID.Extensions;

/// <summary>
///     Provides extension methods for JSON serialization and deserialization.
/// </summary>
public static class JsonExtensions
{
    // ReSharper disable once InconsistentNaming

    /// <summary>
    ///     Gets the predefined <see cref="System.Text.Json.JsonSerializerOptions" /> used for JSON serialization and
    ///     deserialization.
    /// </summary>
    /// <value>
    ///     A <see cref="System.Text.Json.JsonSerializerOptions" /> instance configured with the following settings:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Indented formatting is enabled.</description>
    ///         </item>
    ///         <item>
    ///             <description>Property names are serialized in camel case.</description>
    ///         </item>
    ///         <item>
    ///             <description>Null values are ignored during serialization and deserialization.</description>
    ///         </item>
    ///     </list>
    /// </value>
    public static JsonSerializerOptions Options { get; } = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    ///     Serializes the specified object to a JSON string using predefined serialization options.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize to JSON.</param>
    /// <returns>A JSON string representation of the specified object.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="obj" /> is <c>null</c>.</exception>
    /// <remarks>
    ///     The serialization options include indented formatting, camel case property naming,
    ///     and ignoring null values during serialization.
    /// </remarks>
    public static string ToJson<T>(this T obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }

    /// <summary>
    ///     Deserializes the specified JSON string into an object of type <typeparamref name="T" />
    ///     using predefined deserialization options.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>
    ///     An object of type <typeparamref name="T" /> deserialized from the JSON string,
    ///     or <c>null</c> if the JSON string is invalid or represents a null value.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="json" /> is <c>null</c>.</exception>
    /// <remarks>
    ///     The deserialization options include indented formatting, camel case property naming,
    ///     and ignoring null values during deserialization.
    /// </remarks>
    public static T? FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}