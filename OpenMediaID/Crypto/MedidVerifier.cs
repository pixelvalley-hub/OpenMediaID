using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenMediaID.Models;

namespace OpenMediaID.Crypto;

/// <summary>
///     Provides cryptographic verification functionalities for Medid files within the OpenMediaID system.
/// </summary>
/// <remarks>
///     This static class is responsible for verifying the integrity and authenticity of Medid files by
///     utilizing digital signatures and public key cryptography. It ensures that the data within a Medid file
///     has not been tampered with and is signed by a trusted source.
/// </remarks>
public static class MedidVerifier
{
    /// <summary>
    ///     Verifies the integrity and authenticity of a given <see cref="MedidFile" /> using the provided public key.
    /// </summary>
    /// <param name="medidFile">
    ///     The <see cref="MedidFile" /> instance to verify. This includes the data and its associated digital signature.
    /// </param>
    /// <param name="publicKey">
    ///     The public key, in byte array format, used to verify the digital signature of the <paramref name="medidFile" />.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="medidFile" /> is successfully verified and its data is authentic; otherwise,
    ///     <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     This method ensures that the <paramref name="medidFile" /> has not been tampered with and that its signature
    ///     matches the provided public key. It uses RSA public key cryptography and the SHA-256 hash algorithm
    ///     for verification.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="medidFile" /> or <paramref name="publicKey" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="FormatException">
    ///     Thrown if the signature in the <paramref name="medidFile" /> is not a valid Base64 string.
    /// </exception>
    /// <example>
    ///     <code>
    ///     var medidFile = new MedidFile
    ///     {
    ///         FormatVersion = "1.0",
    ///         Collection = new MediaCollection(),
    ///         Signature = new MedidSignature
    ///         {
    ///             Value = "Base64EncodedSignature",
    ///             Signer = "ExampleSigner",
    ///             PublicKeyHint = "ExampleHint"
    ///         }
    ///     };
    /// 
    ///     byte[] publicKey = Convert.FromBase64String("Base64EncodedPublicKey");
    ///     bool isValid = MedidVerifier.Verify(medidFile, publicKey);
    ///     Console.WriteLine($"Verification result: {isValid}");
    ///     </code>
    /// </example>
    public static bool Verify(MedidFile medidFile, byte[] publicKey)
    {
        if (medidFile.Signature == null || string.IsNullOrEmpty(medidFile.Signature.Value))
            return false;

        var unsigned = medidFile with { Signature = null };
        string json = JsonSerializer.Serialize(unsigned, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        byte[] data = Encoding.UTF8.GetBytes(json);
        byte[] signature = Convert.FromBase64String(medidFile.Signature.Value);

        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

        return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}