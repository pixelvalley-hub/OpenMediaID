using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using OpenMediaID.Crypto;
using OpenMediaID.Extensions;
using OpenMediaID.Models;

namespace OpenMediaID.Packaging;

public static class MedidPackage
{
    /// <summary>
    ///     Loads a <see cref="MedidFile" /> from the specified package file path and extracts the public key if available.
    /// </summary>
    /// <param name="path">The file path of the package to be loaded.</param>
    /// <param name="publicKey">
    ///     When this method returns, contains the public key extracted from the package,
    ///     or <c>null</c> if no public key is found.
    /// </param>
    /// <returns>The deserialized <see cref="MedidFile" /> instance from the package.</returns>
    /// <exception cref="FileNotFoundException">
    ///     Thrown if the specified package file does not exist.
    /// </exception>
    /// <exception cref="JsonException">
    ///     Thrown if the package contains an invalid or malformed JSON file.
    /// </exception>
    /// <remarks>
    ///     This method extracts the contents of the specified package file to a temporary directory,
    ///     reads the public key (if present), deserializes the <c>medid.json</c> file into a <see cref="MedidFile" /> object,
    ///     and then cleans up the temporary directory.
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

    /// <summary>
    ///     Saves the specified <see cref="MedidFile" /> to the given output path as a packaged file.
    /// </summary>
    /// <param name="file">The <see cref="MedidFile" /> instance to be saved.</param>
    /// <param name="outputPath">The file path where the packaged file will be saved.</param>
    /// <param name="options">
    ///     Optional settings for saving the package, such as public/private keys for signing and additional metadata.
    ///     If not provided, default options will be used.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when signing is enabled but <see cref="SaveOptions.SignerName" /> is not set.
    /// </exception>
    /// <remarks>
    ///     This method creates a temporary directory to prepare the package, processes media entries,
    ///     optionally signs the package, and writes the final output as a compressed file.
    /// </remarks>
    public static void Save(MedidFile file, string outputPath, SaveOptions? options = null)
    {
        var opts = options ?? new SaveOptions();

        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        // Optional public.key
        if (opts.PublicKey != null)
            File.WriteAllBytes(Path.Combine(tempDir, "public.key"), opts.PublicKey);

        // Thumbnail-Folder
        string thumbnailDir = Path.Combine(tempDir, "thumbnails");
        Directory.CreateDirectory(thumbnailDir);

        // Media-Folder
        string previewDir = Path.Combine(tempDir, "media");
        Directory.CreateDirectory(previewDir);

        foreach (var entry in file.Collection.Entries)
            if (!string.IsNullOrWhiteSpace(entry.FilePath) && File.Exists(entry.FilePath))
            {
                string safeName = Path.GetFileNameWithoutExtension(entry.Filename);
                // set MIME-Typ via MimeMapping
                entry.MimeType = MimeMapping.GetMimeType(entry.FilePath);

                // Hash (blake3 + optional sha256)
                string blake3 = MediaHasher.ComputeBlake3(entry.FilePath);
                string hash = $"blake3:{blake3}";
                if (opts.IncludeSha256)
                {
                    string sha256 = MediaHasher.ComputeSha256(entry.FilePath);
                    hash = $"sha256:{sha256}|{hash}";
                }

                entry.Hash = hash;

                // Metadata
                entry.Metadata = MediaMetadataExtractor.Extract(entry.FilePath, entry.MimeType);

                // 1. Thumbnail for images
                if (entry.MimeType.StartsWith("image/"))
                {
                    string thumbPath = Path.Combine(thumbnailDir, $"{safeName}_thumb.jpg");
                    try
                    {
                        ThumbnailHelper.CreateThumbnail(entry.FilePath, thumbPath);
                        entry.ThumbnailPath = Path.Combine("thumbnails", Path.GetFileName(thumbPath));
                    }
                    catch
                    {
                        // optional: logging
                    }
                }

                // 2. Preview Media
                if (entry.MimeType.StartsWith("video/"))
                {
                    string previewPath = Path.Combine(previewDir, $"{safeName}_preview.mp4");
                    try
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "ffmpeg",
                                Arguments =
                                    $"-y -i \"{entry.Filename}\" -ss 00:00:01 -t 5 -vf scale=320:-1 -c:v libx264 -an \"{previewPath}\"",
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();

                        if (File.Exists(previewPath))
                            entry.PreviewMediaPath = Path.Combine("media", Path.GetFileName(previewPath));
                    }
                    catch
                    {
                        // optional: logging
                    }
                }
            }

        // Optional: Signing
        if (opts.PrivateKey != null && !string.IsNullOrWhiteSpace(opts.SignerName))
        {
            if (string.IsNullOrWhiteSpace(opts.SignerName))
                throw new InvalidOperationException("SignerName must be set if PrivateKey is provided.");

            Trace.WriteLine("🔏 Signing medid file...");
            file = MedidSigner.Sign(file, opts.PrivateKey, opts.SignerName, opts.PublicKeyHint ?? "default-key");
        }

        // Final: write medid.json
        string jsonPath = Path.Combine(tempDir, "medid.json");
        File.WriteAllText(jsonPath, file.ToJson());

        // Create Package
        if (File.Exists(outputPath)) File.Delete(outputPath);
        ZipFile.CreateFromDirectory(tempDir, outputPath);

        Directory.Delete(tempDir, true);
    }

    /// <summary>
    ///     Attempts to save the specified <see cref="MedidFile" /> to the given output path.
    /// </summary>
    /// <param name="file">
    ///     The <see cref="MedidFile" /> instance to be saved.
    /// </param>
    /// <param name="outputPath">
    ///     The file path where the package should be saved.
    /// </param>
    /// <param name="errors">
    ///     When this method returns, contains a list of error messages if the operation fails.
    /// </param>
    /// <param name="options">
    ///     Optional <see cref="SaveOptions" /> to customize the save operation. If not provided, default options are used.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the package was successfully saved; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     This method validates the structure of the <see cref="MedidFile" /> before attempting to save it.
    ///     If validation fails or an exception occurs during the save operation, the method returns <c>false</c>
    ///     and populates the <paramref name="errors" /> parameter with relevant error messages.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="file" /> or <paramref name="outputPath" /> is <c>null</c>.
    /// </exception>
    /// <example>
    ///     <code>
    /// var medidFile = new MedidFile { /* Initialize properties */ };
    /// var errors = new List<string>
    ///             ();
    ///             var success = MedidPackage.TrySavePackage(medidFile, "outputPath.zip", out errors);
    ///             if (!success)
    ///             {
    ///             Console.WriteLine("Failed to save package:");
    ///             foreach (var error in errors)
    ///             {
    ///             Console.WriteLine(error);
    ///             }
    ///             }
    /// </code>
    /// </example>
    public static bool TrySavePackage(MedidFile file, string outputPath, out List<string> errors,
        SaveOptions? options = null)
    {
        errors = new List<string>();
        if (!ValidateStructureOnly(file, out errors))
            return false;

        try
        {
            Save(file, outputPath, options);
            return true;
        }
        catch (Exception ex)
        {
            errors.Add("Error while saving package: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    ///     Validates the structural integrity of a <see cref="MedidFile" /> instance without performing any additional
    ///     operations.
    /// </summary>
    /// <param name="file">
    ///     The <see cref="MedidFile" /> instance to validate. This parameter must not be <c>null</c>.
    /// </param>
    /// <param name="errors">
    ///     When this method returns, contains a list of validation errors, if any were found.
    ///     If no errors are found, this list will be empty.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the structure of the <paramref name="file" /> is valid; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     This method checks for the presence of required properties and ensures that all entries in the media collection
    ///     meet the necessary criteria. It does not perform any file I/O or cryptographic operations.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="file" /> parameter is <c>null</c>.
    /// </exception>
    public static bool ValidateStructureOnly(MedidFile file, out List<string> errors)
    {
        errors = new List<string>();

        if (file == null)
        {
            errors.Add("MedidFile is null.");
            return false;
        }

        if (file.Collection == null)
        {
            errors.Add("Missing collection.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(file.Collection.Name))
            errors.Add("Collection name is required.");

        if (file.Collection.Entries == null || file.Collection.Entries.Count == 0)
            errors.Add("At least one media entry is required.");

        foreach (var entry in file.Collection.Entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Filename))
                errors.Add("Filename is missing in one or more entries.");

            if (string.IsNullOrWhiteSpace(entry.MimeType))
                errors.Add($"Entry '{entry.Filename}' is missing MimeType.");

            if (string.IsNullOrWhiteSpace(entry.Hash))
                errors.Add($"Entry '{entry.Filename}' is missing Hash.");

            if (entry.Metadata == null)
                errors.Add($"Entry '{entry.Filename}' is missing Metadata.");
        }

        return errors.Count == 0;
    }
}