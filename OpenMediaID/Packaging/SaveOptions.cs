namespace OpenMediaID.Packaging;

/// <summary>
///     Represents the options for saving media packaging configurations.
/// </summary>
public class SaveOptions
{
    /// <summary>
    ///     Gets or sets the public key used for signing or encryption purposes.
    /// </summary>
    /// <value>
    ///     A byte array representing the public key, or <c>null</c> if no public key is set.
    /// </value>
    public byte[]? PublicKey { get; set; }

    /// <summary>
    ///     Gets or sets the private key used for signing or encrypting data.
    /// </summary>
    /// <remarks>
    ///     This property holds the private key as a byte array. Ensure that the key is securely managed
    ///     and not exposed to unauthorized access. It is typically used in conjunction with the
    ///     <see cref="PublicKey" /> property for cryptographic operations.
    /// </remarks>
    public byte[]? PrivateKey { get; set; }

    /// <summary>
    ///     Gets or sets the name of the signer associated with the save options.
    /// </summary>
    /// <value>
    ///     A <see cref="string" /> representing the name of the signer. Can be <c>null</c>.
    /// </value>
    public string? SignerName { get; set; }

    /// <summary>
    ///     Gets or sets a hint or identifier associated with the public key.
    /// </summary>
    /// <remarks>
    ///     This property can be used to provide additional context or metadata
    ///     about the public key, such as a descriptive name or identifier.
    /// </remarks>
    public string? PublicKeyHint { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the SHA-256 hash should be included during the save operation.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the SHA-256 hash should be included; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeSha256 { get; set; } = false;

    /// <summary>
    ///     Gets or sets a value indicating whether thumbnails should be included during the save operation.
    /// </summary>
    /// <value>
    ///     <c>true</c> if thumbnails should be included; otherwise, <c>false</c>.
    ///     The default value is <c>true</c>.
    /// </value>
    public bool IncludeThumbnails { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether preview media should be included during the save operation.
    /// </summary>
    /// <value>
    ///     <c>true</c> if preview media should be included; otherwise, <c>false</c>.
    /// </value>
    public bool IncludePreviewMedia { get; set; } = true;
}