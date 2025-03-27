using System.IO.Compression;
using System.Text.Json;
using OpenMediaID.Models;

namespace OpenMediaID.Packaging;

/// <summary>
///     Provides functionality for packaging and unpackaging <see cref="OpenMediaID.Models.MedidFile" /> objects.
/// </summary>
/// <remarks>
///     This static class includes methods to save a <see cref="OpenMediaID.Models.MedidFile" /> to a compressed package
///     and to load it back, optionally handling associated public keys. It facilitates the serialization and
///     deserialization of Medid files, ensuring their integrity and portability within the OpenMediaID system.
/// </remarks>
public static class MedidPackage
{
    /// <summary>
    ///     Saves the specified <see cref="OpenMediaID.Models.MedidFile" /> to a compressed package at the given output path.
    /// </summary>
    /// <param name="file">
    ///     The <see cref="OpenMediaID.Models.MedidFile" /> to be saved.
    /// </param>
    /// <param name="outputPath">
    ///     The file path where the compressed package will be created.
    /// </param>
    /// <param name="publicKey">
    ///     An optional public key to include in the package. If provided, it will be saved as "public.key".
    /// </param>
    /// <remarks>
    ///     This method serializes the <see cref="OpenMediaID.Models.MedidFile" /> into a JSON file, optionally includes a
    ///     public key,
    ///     and compresses the data into a ZIP package. Temporary files are created during the process and are cleaned up
    ///     afterward.
    /// </remarks>
    /// <exception cref="System.IO.IOException">
    ///     Thrown if an I/O error occurs during the creation of the package.
    /// </exception>
    /// <exception cref="System.UnauthorizedAccessException">
    ///     Thrown if the application lacks the necessary permissions to access the specified paths.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///     Thrown if the <paramref name="outputPath" /> is invalid.
    /// </exception>
    public static void Save(MedidFile file, string outputPath, byte[]? publicKey = null)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        string jsonPath = Path.Combine(tempDir, "medid.json");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(file, new JsonSerializerOptions { WriteIndented = true }));

        // Optional public.key
        if (publicKey != null)
            File.WriteAllBytes(Path.Combine(tempDir, "public.key"), publicKey);

        if (File.Exists(outputPath)) File.Delete(outputPath);
        ZipFile.CreateFromDirectory(tempDir, outputPath);

        Directory.Delete(tempDir, true);
    }

    /// <summary>
    ///     Loads a <see cref="OpenMediaID.Models.MedidFile" /> from a compressed package and retrieves the associated public
    ///     key, if available.
    /// </summary>
    /// <param name="path">
    ///     The file path to the compressed package containing the Medid file.
    /// </param>
    /// <param name="publicKey">
    ///     When this method returns, contains the public key as a byte array if it exists in the package; otherwise,
    ///     <c>null</c>.
    /// </param>
    /// <returns>
    ///     A <see cref="OpenMediaID.Models.MedidFile" /> instance deserialized from the package.
    /// </returns>
    /// <exception cref="System.IO.FileNotFoundException">
    ///     Thrown if the specified file path does not exist.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    ///     Thrown if an I/O error occurs during the extraction or reading process.
    /// </exception>
    /// <exception cref="System.Text.Json.JsonException">
    ///     Thrown if the JSON content in the package is invalid or cannot be deserialized into a
    ///     <see cref="OpenMediaID.Models.MedidFile" />.
    /// </exception>
    /// <remarks>
    ///     This method extracts the contents of the specified package to a temporary directory, reads the Medid file
    ///     and optionally retrieves the public key if it is included in the package. The temporary directory is deleted
    ///     after the operation completes.
    /// </remarks>
    public static MedidFile Load(string path, out byte[]? publicKey)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ZipFile.ExtractToDirectory(path, tempDir);

        string pubPath = Path.Combine(tempDir, "public.key");
        publicKey = File.Exists(pubPath) ? File.ReadAllBytes(pubPath) : null;

        string json = File.ReadAllText(Path.Combine(tempDir, "medid.json"));
        var medid = JsonSerializer.Deserialize<MedidFile>(json)!;

        Directory.Delete(tempDir, true);
        return medid;
    }
}