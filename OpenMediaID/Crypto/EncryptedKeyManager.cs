using System.Security.Cryptography;

namespace OpenMediaID.Crypto;

/// <summary>
///     Provides functionality for encrypting and decrypting private keys using AES encryption.
///     This class includes methods to securely encrypt private keys with a password and
///     decrypt them back to their original form.
/// </summary>
public static class EncryptedKeyManager
{
    private const int Iterations = 100_000;
    private const int KeySizeBytes = 32; // 256 bit

    /// <summary>
    ///     Encrypts a private key using AES encryption and a password.
    /// </summary>
    /// <param name="privateKey">The private key to encrypt, represented as a byte array.</param>
    /// <param name="password">The password used to derive the encryption key.</param>
    /// <returns>
    ///     A byte array containing the encrypted private key. The format of the output is:
    ///     [salt][IV][cipher], where:
    ///     - <c>salt</c>: A 16-byte random salt used for key derivation.
    ///     - <c>IV</c>: A 16-byte initialization vector used for AES encryption.
    ///     - <c>cipher</c>: The encrypted private key.
    /// </returns>
    /// <remarks>
    ///     This method uses a randomly generated salt and initialization vector (IV) to enhance security.
    ///     The password is used to derive a 256-bit encryption key using PBKDF2 with 100,000 iterations
    ///     and SHA-256 as the hash algorithm.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="privateKey" /> or <paramref name="password" /> is <c>null</c>.
    /// </exception>
    /// <example>
    ///     <code>
    /// byte[] privateKey = Encoding.UTF8.GetBytes("my-private-key");
    /// string password = "secure-password";
    /// byte[] encryptedKey = EncryptedKeyManager.EncryptPrivateKey(privateKey, password);
    /// </code>
    /// </example>
    public static byte[] EncryptPrivateKey(byte[] privateKey, string password)
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateIV();

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var key = DeriveKey(password, salt);

        aes.Key = key;
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(salt); // 16 byte
        writer.Write(aes.IV); // 16 byte

        using var crypto = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        crypto.Write(privateKey, 0, privateKey.Length);
        crypto.FlushFinalBlock();

        return ms.ToArray(); // => [salt][IV][cipher]
    }

    /// <summary>
    ///     Decrypts an encrypted private key using AES decryption and a password.
    /// </summary>
    /// <param name="encryptedData">
    ///     The encrypted private key data, represented as a byte array. The format of the input is:
    ///     [salt][IV][cipher], where:
    ///     - <c>salt</c>: A 16-byte random salt used for key derivation.
    ///     - <c>IV</c>: A 16-byte initialization vector used for AES decryption.
    ///     - <c>cipher</c>: The encrypted private key.
    /// </param>
    /// <param name="password">The password used to derive the decryption key.</param>
    /// <returns>
    ///     A byte array containing the decrypted private key.
    /// </returns>
    /// <remarks>
    ///     This method uses the salt and initialization vector (IV) embedded in the encrypted data
    ///     to derive the decryption key and decrypt the private key. The password is used to derive
    ///     a 256-bit decryption key using PBKDF2 with 100,000 iterations and SHA-256 as the hash algorithm.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="encryptedData" /> or <paramref name="password" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="CryptographicException">
    ///     Thrown if the decryption process fails, such as when the password is incorrect or the data is corrupted.
    /// </exception>
    /// <example>
    ///     <code>
    /// byte[] encryptedData = GetEncryptedData(); // Retrieve the encrypted private key
    /// string password = "secure-password";
    /// byte[] privateKey = EncryptedKeyManager.DecryptPrivateKey(encryptedData, password);
    /// </code>
    /// </example>
    public static byte[] DecryptPrivateKey(byte[] encryptedData, string password)
    {
        using var ms = new MemoryStream(encryptedData);
        using var reader = new BinaryReader(ms);

        byte[] salt = reader.ReadBytes(16);
        byte[] iv = reader.ReadBytes(16);
        byte[] cipher = reader.ReadBytes((int)(ms.Length - 32));

        var key = DeriveKey(password, salt);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.IV = iv;
        aes.Key = key;

        using var crypto = new CryptoStream(new MemoryStream(cipher), aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var result = new MemoryStream();
        crypto.CopyTo(result);

        return result.ToArray();
    }

    /// <summary>
    ///     Derives a cryptographic key from a password and a salt using PBKDF2 (Password-Based Key Derivation Function 2).
    /// </summary>
    /// <param name="password">The password used to derive the key.</param>
    /// <param name="salt">The salt used in the key derivation process, represented as a byte array.</param>
    /// <returns>
    ///     A byte array containing the derived cryptographic key. The key size is 256 bits (32 bytes).
    /// </returns>
    /// <remarks>
    ///     This method uses the PBKDF2 algorithm with the following parameters:
    ///     - Hash algorithm: SHA-256.
    ///     - Iteration count: 100,000.
    ///     - Key size: 256 bits (32 bytes).
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="password" /> or <paramref name="salt" /> is <c>null</c>.
    /// </exception>
    private static byte[] DeriveKey(string password, byte[] salt)
    {
        using var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return derive.GetBytes(KeySizeBytes);
    }
}