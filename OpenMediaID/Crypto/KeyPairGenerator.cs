using System.Security.Cryptography;

namespace OpenMediaID.Crypto;

/// <summary>
///     Provides functionality to generate cryptographic key pairs.
/// </summary>
public static class KeyPairGenerator
{
    /// <summary>
    ///     Generates a cryptographic key pair consisting of a private key and a public key.
    /// </summary>
    /// <param name="keySize">
    ///     The size of the key to generate, in bits. The default value is 2048.
    /// </param>
    /// <returns>
    ///     A tuple containing the private key and public key as byte arrays.
    ///     The private key is suitable for signing, and the public key is used for validation.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the specified <paramref name="keySize" /> is not supported.
    /// </exception>
    public static (byte[] PrivateKey, byte[] PublicKey) Generate(int keySize = 2048)
    {
        using var rsa = RSA.Create(keySize);

        byte[] privateKey = rsa.ExportPkcs8PrivateKey(); // for Signature
        byte[] publicKey = rsa.ExportSubjectPublicKeyInfo(); // for validation

        return (privateKey, publicKey);
    }
}