using System.Text.Json.Serialization;

namespace OpenMediaID.Models;

/// <summary>
///     Represents a Medid file within the OpenMediaID system, containing information about the format version,
///     a collection of media entries, and an optional digital signature.
/// </summary>
/// <remarks>
///     This record serves as the primary structure for encapsulating media-related data in the OpenMediaID system.
///     It includes details about the format version, the associated media collection, and an optional signature
///     for verifying the integrity and authenticity of the file.
/// </remarks>
public record MedidFile
{
    /// <summary>
    ///     Gets the version of the Medid file format.
    /// </summary>
    /// <remarks>
    ///     This property specifies the version of the Medid file format being used.
    ///     It is primarily intended to ensure compatibility between different versions
    ///     of the OpenMediaID system.
    /// </remarks>
    [JsonPropertyName("medid")]
    public string FormatVersion { get; init; } = "1.0";

    /// <summary>
    ///     Gets the collection of media entries associated with this Medid file.
    /// </summary>
    /// <remarks>
    ///     This property represents the media collection contained within the Medid file,
    ///     including metadata such as the collection's name, publisher, creation date,
    ///     hash algorithm, and the list of media entries it encompasses.
    /// </remarks>
    [JsonPropertyName("collection")]
    public required MediaCollection Collection { get; init; }

    /// <summary>
    ///     Gets or sets the optional digital signature associated with the Medid file.
    /// </summary>
    /// <remarks>
    ///     The signature is used to verify the integrity and authenticity of the Medid file.
    ///     It includes details such as the signature value, the signer, and an optional public key hint.
    /// </remarks>
    [JsonPropertyName("signature")]
    public MedidSignature? Signature { get; set; }
}