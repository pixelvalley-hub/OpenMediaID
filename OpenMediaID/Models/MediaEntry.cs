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
    ///     Gets the hash value of the media file, which serves as a unique identifier for its content.
    /// </summary>
    [JsonPropertyName("hash")]
    public required string Hash { get; init; }

    /// <summary>
    ///     Gets the size of the media file in bytes.
    /// </summary>
    /// <remarks>
    ///     This property represents the total size of the media file, measured in bytes.
    /// </remarks>
    [JsonPropertyName("length")]
    public long LengthInBytes { get; init; }

    /// <summary>
    ///     Gets the metadata associated with the media file, such as its dimensions, duration, and other properties.
    /// </summary>
    /// <value>
    ///     An instance of <see cref="MediaMetadata" /> containing detailed metadata about the media file.
    /// </value>
    [JsonPropertyName("metadata")]
    public required MediaMetadata Metadata { get; init; }

    /// <summary>
    ///     Gets the MIME type of the media file, which indicates the format of the file (e.g., "image/jpeg", "video/mp4").
    /// </summary>
    [JsonPropertyName("mimeType")]
    public required string MimeType { get; init; } //

    /// <summary>
    ///     Gets or sets the optional path to the thumbnail image associated with the media file.
    /// </summary>
    /// <remarks>
    ///     This property may contain a relative path, such as "thumbnails/video1.jpg",
    ///     or be <c>null</c> if no thumbnail is available.
    /// </remarks>
    [JsonPropertyName("thumbnailPath")]
    public string? ThumbnailPath { get; init; }

    /// <summary>
    ///     Gets or sets the optional path to the preview version of the media file.
    /// </summary>
    /// <remarks>
    ///     This property contain a relative path, such as "media/video1_preview.mp4", or be <c>null</c> if no preview media is
    ///     available.
    /// </remarks>
    [JsonPropertyName("previewMediaPath")]
    public string? PreviewMediaPath { get; init; }
}