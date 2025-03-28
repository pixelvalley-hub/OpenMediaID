namespace OpenMediaID.Packaging;

/// <summary>
///     Provides functionality for determining the MIME type of a file based on its file extension.
/// </summary>
public static class MimeMapping
{
    /// <summary>
    ///     Determines the MIME type of a file based on its file extension.
    /// </summary>
    /// <param name="filePath">The path of the file for which to determine the MIME type.</param>
    /// <returns>
    ///     A string representing the MIME type of the file. If the file extension is not recognized,
    ///     the default MIME type "application/octet-stream" is returned.
    /// </returns>
    /// <remarks>
    ///     This method uses the file extension to infer the MIME type. Common extensions like
    ///     ".jpg", ".png", ".mp4", etc., are mapped to their respective MIME types.
    ///     Unrecognized extensions default to "application/octet-stream".
    /// </remarks>
    public static string GetMimeType(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            _ => "application/octet-stream"
        };
    }
}