using System.Security.Cryptography;
using System.Text;
using OpenMediaID.Extensions;
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
    ///     The <see cref="OpenMediaID.Models.MedidFile" /> to be signed. The signature will be added to this file.
    /// </param>
    /// <param name="privateKey">
    ///     The RSA private key in PKCS#8 format used to generate the digital signature.
    /// </param>
    /// <param name="signerName">
    ///     The name of the signer, which will be included in the signature metadata.
    /// </param>
    /// <param name="publicKeyHint">
    ///     A hint or identifier for the corresponding public key, aiding in signature verification.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="OpenMediaID.Models.MedidFile" /> with the generated digital signature included.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="medidFile" />, <paramref name="privateKey" />, or <paramref name="signerName" /> is
    ///     <c>null</c>.
    /// </exception>
    /// <exception cref="CryptographicException">
    ///     Thrown if the private key is invalid or if an error occurs during the signing process.
    /// </exception>
    /// <remarks>
    ///     This method ensures the integrity and authenticity of the <see cref="OpenMediaID.Models.MedidFile" /> by generating
    ///     a digital signature using RSA cryptography. The signature is computed over the JSON representation of the file
    ///     (excluding any existing signature) and is stored in the <see cref="OpenMediaID.Models.MedidFile.Signature" />
    ///     property.
    /// </remarks>
    public static MedidFile Sign(MedidFile medidFile, byte[] privateKey, string signerName, string publicKeyHint)
    {
        var unsigned = medidFile with { Signature = null };

        byte[] data = Encoding.UTF8.GetBytes(unsigned.ToJson());

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