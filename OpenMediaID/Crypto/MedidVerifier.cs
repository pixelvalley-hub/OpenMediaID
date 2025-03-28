using OpenMediaID.Extensions;
using OpenMediaID.Models;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OpenMediaID.Crypto;

/// <summary>
/// Provides methods for verifying the authenticity and integrity of <see cref="MedidFile"/> instances 
/// and related package files.
/// </summary>
/// <remarks>
/// This class includes functionality to ensure that files and packages have not been tampered with 
/// and originate from a trusted source by validating their digital signatures.
/// </remarks>
public static class MedidVerifier
{
    /// <summary>
    /// Verifies the authenticity and integrity of a <see cref="MedidFile"/> using the provided public key.
    /// </summary>
    /// <param name="medidFile">The <see cref="MedidFile"/> to verify.</param>
    /// <param name="publicKey">
    /// A byte array representing the public key used to verify the digital signature of the <paramref name="medidFile"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="medidFile"/> is successfully verified; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks the digital signature of the specified <paramref name="medidFile"/> against the provided public key.
    /// It ensures that the file has not been tampered with and that it originates from a trusted source.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown if <paramref name="medidFile"/> or <paramref name="publicKey"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.FormatException">
    /// Thrown if the signature in the <paramref name="medidFile"/> is not a valid Base64 string.
    /// </exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">
    /// Thrown if there is an error during the cryptographic operations, such as importing the public key.
    /// </exception>
    public static bool Verify(MedidFile medidFile, byte[] publicKey)
    {
        if (medidFile.Signature == null || string.IsNullOrEmpty(medidFile.Signature.Value))
            return false;

        var unsigned = medidFile with { Signature = null };

        byte[] data = Encoding.UTF8.GetBytes(unsigned.ToJson());
        byte[] signature = Convert.FromBase64String(medidFile.Signature.Value);

        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

        return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    /// <summary>
    /// Verifies the authenticity and integrity of a package located at the specified path.
    /// </summary>
    /// <param name="packagePath">
    /// The file path to the package to verify. The package is expected to be a ZIP archive containing
    /// a "medid.json" file and a "public.key" file.
    /// </param>
    /// <returns>
    /// <c>true</c> if the package is successfully verified; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method extracts the package, reads the "medid.json" file and the "public.key" file, and verifies
    /// the digital signature of the <see cref="MedidFile"/> contained in the package using the public key.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// Thrown if <paramref name="packagePath"/> is <c>null</c>, empty, or consists only of white-space characters.
    /// </exception>
    /// <exception cref="System.IO.FileNotFoundException">
    /// Thrown if the file specified by <paramref name="packagePath"/> does not exist.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// Thrown if an I/O error occurs while extracting the package or reading its contents.
    /// </exception>
    /// <exception cref="System.Text.Json.JsonException">
    /// Thrown if the "medid.json" file is not a valid JSON or cannot be deserialized into a <see cref="MedidFile"/>.
    /// </exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">
    /// Thrown if there is an error during the cryptographic operations, such as importing the public key.
    /// </exception>
    public static bool VerifyPackage(string packagePath)
    {
        if (string.IsNullOrWhiteSpace(packagePath) || !File.Exists(packagePath))
            return false;

        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            ZipFile.ExtractToDirectory(packagePath, tempDir);

            string jsonPath = Path.Combine(tempDir, "medid.json");
            string keyPath = Path.Combine(tempDir, "public.key");

            if (!File.Exists(jsonPath) || !File.Exists(keyPath))
                return false;

            string json = File.ReadAllText(jsonPath);
            byte[] publicKey = File.ReadAllBytes(keyPath);
            MedidFile medid = JsonSerializer.Deserialize<MedidFile>(json)!;

            return Verify(medid, publicKey);
        }
        catch
        {
            return false;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}