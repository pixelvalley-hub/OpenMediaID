using System.Text.Json.Serialization;

namespace OpenMediaID.Models;

/// <summary>
///     Represents a collection of media entries, including metadata such as the collection's name, publisher,
///     creation date, hash algorithm used, and the list of media entries it contains.
/// </summary>
public record MediaCollection
{
    /// <summary>
    ///     Gets the name of the media collection.
    /// </summary>
    /// <remarks>
    ///     This property represents the name assigned to the media collection, which can be used to identify or describe it.
    /// </remarks>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Gets the name of the publisher associated with the media collection.
    /// </summary>
    [JsonPropertyName("publisher")]
    public string? Publisher { get; init; }

    /// <summary>
    ///     Gets the date and time when the media collection was created.
    /// </summary>
    /// <remarks>
    ///     This property represents the creation timestamp of the media collection and is serialized
    ///     using the JSON property name "created".
    /// </remarks>
    [JsonPropertyName("created")]
    public DateTime Created { get; init; }

    /// <summary>
    ///     Gets the name of the hash algorithm used for generating hashes of media entries in the collection.
    ///     Defaults to "blake3".
    /// </summary>
    /// <remarks>
    ///     The hash algorithm is used to ensure the integrity and uniqueness of media entries.
    ///     This property is serialized as "hashAlgorithm" in JSON.
    /// </remarks>
    [JsonPropertyName("hashAlgorithm")]
    public string HashAlgorithm { get; init; } = "blake3";

    /// <summary>
    ///     Gets the list of media entries contained in the collection. Each entry represents a media file with
    ///     associated metadata, such as filename, hash, size, MIME type, and optional paths for thumbnail and preview media.
    /// </summary>
    [JsonPropertyName("entries")]
    public List<MediaEntry> Entries { get; init; } = [];
}