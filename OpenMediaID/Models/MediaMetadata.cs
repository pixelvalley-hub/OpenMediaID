using System.Text.Json.Serialization;

namespace OpenMediaID.Models;

/// <summary>
///     Represents metadata associated with a media file, including dimensions, duration, and other properties.
/// </summary>
public record MediaMetadata
{
    /// <summary>
    ///     Gets the width of the media file in pixels.
    /// </summary>
    /// <remarks>
    ///     This property represents the horizontal dimension of the media file.
    ///     The value is nullable, indicating that the width may not always be available.
    /// </remarks>
    [JsonPropertyName("width")]
    public int? Width { get; init; }

    /// <summary>
    ///     Gets or initializes the height of the media in pixels.
    /// </summary>
    /// <remarks>
    ///     This property represents the vertical dimension of the media.
    ///     A value of <c>null</c> indicates that the height is unspecified.
    /// </remarks>
    [JsonPropertyName("height")]
    public int? Height { get; init; }

    /// <summary>
    ///     Gets or initializes the duration of the media file.
    /// </summary>
    /// <remarks>
    ///     The duration is represented as a string and may follow a specific format,
    ///     such as "hh:mm:ss" or another convention depending on the context.
    /// </remarks>
    [JsonPropertyName("duration")]
    public string? Duration { get; init; }

    /// <summary>
    ///     Gets or initializes the date and time when the media was taken or created.
    /// </summary>
    /// <remarks>
    ///     This property represents the timestamp associated with the media file's creation or capture.
    ///     The value is nullable, indicating that the date may not always be available.
    /// </remarks>
    [JsonPropertyName("dateTaken")]
    public DateTime? DateTaken { get; init; }
}