using System.Security.Cryptography;
using Blake3;

namespace OpenMediaID.Packaging;

/// <summary>
///     Provides functionality for computing cryptographic hashes of files using various algorithms,
///     including SHA-256 and BLAKE3.
/// </summary>
public class MediaHasher
{
    /// <summary>
    ///     Computes the SHA-256 hash of the specified file.
    /// </summary>
    /// <param name="filePath">The path to the file for which the SHA-256 hash is to be computed.</param>
    /// <returns>
    ///     A <see cref="string" /> representing the computed SHA-256 hash in lowercase hexadecimal format.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file specified by <paramref name="filePath" /> does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if access to the file is denied.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs while accessing the file.</exception>
    public static string ComputeSha256(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    ///     Computes the BLAKE3 hash of the specified file.
    /// </summary>
    /// <param name="filePath">The path to the file for which the BLAKE3 hash is to be computed.</param>
    /// <returns>
    ///     A <see cref="string" /> representing the computed BLAKE3 hash in lowercase hexadecimal format.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file specified by <paramref name="filePath" /> does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if access to the file is denied.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs while accessing the file.</exception>
    public static string ComputeBlake3(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        var hash = Hasher.Hash(fileBytes);
        return hash.ToString(); // 64 Hex
    }
}