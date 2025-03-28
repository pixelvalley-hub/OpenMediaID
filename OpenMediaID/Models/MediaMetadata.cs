using System.Text.Json.Serialization;

namespace OpenMediaID.Models;

/// <summary>
///     Represents metadata associated with a media file, including dimensions, duration, and other properties.
/// </summary>
public record MediaMetadata
{
    /// <summary>
    ///     Gets or sets the width of the media in pixels. This property is optional and may be null if the width is not
    ///     available.
    /// </summary>
    /// <remarks>
    ///     This property is typically populated when the media is an image or video.
    /// </remarks>
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>
    ///     Gets or sets the height of the media in pixels.
    /// </summary>
    /// <remarks>
    ///     This property represents the vertical dimension of the media file.
    ///     It is nullable to account for cases where the height is unknown or not applicable.
    /// </remarks>
    [JsonPropertyName("height")]
    public int? Height { get; set; }

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