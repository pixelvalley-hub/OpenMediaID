using System.Text.Json.Serialization;

namespace OpenMediaID.Models;

/// <summary>
///     Represents a media entry containing information about a media file, such as its filename, hash, size, metadata,
///     MIME type, and optional paths for thumbnail and preview media.
/// </summary>
public record MediaEntry
{
    /// <summary>
    ///     Gets or initializes the filename of the media file.
    /// </summary>
    /// <remarks>
    ///     The filename represents the name of the media file, including its extension.
    ///     This property is required and is serialized as "filename" in JSON.
    /// </remarks>
    [JsonPropertyName("filename")]
    public required string Filename { get; init; }

    /// <summary>
    ///     Gets or sets the hash value of the media file, which is used to uniquely identify its content.
    ///     Defaults to "unknown" if not explicitly set.
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = "unknown";

    /// <summary>
    ///     Gets the size of the media file in bytes.
    /// </summary>
    /// <remarks>
    ///     This property represents the total size of the media file, measured in bytes.
    /// </remarks>
    [JsonPropertyName("length")]
    public long LengthInBytes { get; init; }

    /// <summary>
    ///     Gets or sets the metadata associated with the media entry, such as dimensions, duration, and other properties.
    /// </summary>
    [JsonPropertyName("metadata")]
    public MediaMetadata Metadata { get; set; } = new();

    /// <summary>
    ///     Gets or sets the file path of the media entry. This property is ignored during JSON serialization.
    /// </summary>
    [JsonIgnore]
    public string? FilePath { get; set; }

    /// <summary>
    ///     Gets or sets the MIME type of the media file.
    /// </summary>
    /// <remarks>
    ///     The MIME type is used to indicate the format of the media file, such as "image/jpeg" or "video/mp4".
    ///     Defaults to "application/unknown" if not explicitly set.
    /// </remarks>
    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; } = "application/unknown";

    /// <summary>
    ///     Gets or sets the optional path to the thumbnail image associated with the media file.
    /// </summary>
    /// <remarks>
    ///     This property may be <see langword="null" /> if no thumbnail is available.
    /// </remarks>
    [JsonPropertyName("thumbnailPath")]
    public string? ThumbnailPath { get; set; }

    /// <summary>
    ///     Gets or sets the optional path to the preview media associated with the media entry.
    /// </summary>
    /// <remarks>
    ///     This property provides a path to a preview version of the media file, which can be used for quick viewing or
    ///     reduced-size display purposes. It is serialized as "previewMediaPath" in JSON.
    /// </remarks>
    [JsonPropertyName("previewMediaPath")]
    public string? PreviewMediaPath { get; set; }
}