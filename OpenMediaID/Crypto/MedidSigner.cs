using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenMediaID.Models;

namespace OpenMediaID.Crypto;

/// <summary>
///     Provides cryptographic signing functionality for Medid files within the OpenMediaID system.
/// </summary>
/// <remarks>
///     This static class is responsible for generating digital signatures for <see cref="OpenMediaID.Models.MedidFile" />
///     instances. It utilizes RSA cryptography to ensure the integrity and authenticity of the signed files.
/// </remarks>
public static class MedidSigner
{
    /// <summary>
    ///     Signs the specified <see cref="OpenMediaID.Models.MedidFile" /> using the provided private key and signer details.
    /// </summary>
    /// <param name="medidFile">
    ///     The <see cref="OpenMediaID.Models.MedidFile" /> instance to be signed. Its
    ///     <see cref="OpenMediaID.Models.MedidFile.Signature" />
    ///     property will be replaced with the generated signature.
    /// </param>
    /// <param name="privateKey">
    ///     The private key in PKCS#8 format used to generate the digital signature.
    /// </param>
    /// <param name="signerName">
    ///     The name of the signer to be included in the signature metadata.
    /// </param>
    /// <param name="publicKeyHint">
    ///     An optional hint for identifying the public key associated with the private key.
    /// </param>
    /// <returns>
    ///     A new <see cref="OpenMediaID.Models.MedidFile" /> instance with the
    ///     <see cref="OpenMediaID.Models.MedidFile.Signature" />
    ///     property populated with the generated signature.
    /// </returns>
    /// <remarks>
    ///     This method serializes the provided <see cref="OpenMediaID.Models.MedidFile" /> (excluding its signature), computes
    ///     a digital signature using RSA with SHA-256, and returns a new instance of the file with the signature applied.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown if <paramref name="medidFile" />, <paramref name="privateKey" />, or <paramref name="signerName" /> is
    ///     <c>null</c>.
    /// </exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">
    ///     Thrown if an error occurs during the signing process, such as an invalid private key.
    /// </exception>
    public static MedidFile Sign(MedidFile medidFile, byte[] privateKey, string signerName, string publicKeyHint)
    {
        var unsigned = medidFile with { Signature = null };

        string jsonToSign = JsonSerializer.Serialize(unsigned, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        byte[] data = Encoding.UTF8.GetBytes(jsonToSign);

        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(privateKey, out _);

        byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        string signatureBase64 = Convert.ToBase64String(signature);

        var signatureObject = new MedidSignature
        {
            Value = signatureBase64,
            Signer = signerName,
            PublicKeyHint = publicKeyHint
        };

        return medidFile with { Signature = signatureObject };
    }
}