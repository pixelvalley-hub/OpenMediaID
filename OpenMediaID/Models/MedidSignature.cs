using System.Text.Json.Serialization;

namespace OpenMediaID.Models;

/// <summary>
///     Represents a digital signature within the OpenMediaID system.
/// </summary>
/// <remarks>
///     This record encapsulates the essential details of a signature, including its value,
///     the signer, and an optional hint for the public key associated with the signature.
/// </remarks>
public record MedidSignature
{
    /// <summary>
    ///     Gets the value of the digital signature.
    /// </summary>
    /// <remarks>
    ///     This property holds the actual signature data in string format. It is a required field
    ///     and is used to verify the integrity and authenticity of the associated data.
    /// </remarks>
    [JsonPropertyName("value")]
    public required string Value { get; init; }

    /// <summary>
    ///     Gets or sets the identifier of the signer associated with the digital signature.
    /// </summary>
    /// <remarks>
    ///     This property represents the signer of the digital signature. It can be null if the signer is not specified.
    /// </remarks>
    [JsonPropertyName("signer")]
    public string? Signer { get; init; }

    /// <summary>
    ///     Gets or initializes an optional hint for the public key associated with the signature.
    /// </summary>
    /// <remarks>
    ///     This property provides additional context or metadata about the public key
    ///     that can be used to verify the signature. It is optional and may be null.
    /// </remarks>
    [JsonPropertyName("publicKeyHint")]
    public string? PublicKeyHint { get; init; }
}