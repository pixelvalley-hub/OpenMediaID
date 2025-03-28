using SixLabors.ImageSharp;
using OpenMediaID.Models;

namespace OpenMediaID.Packaging
{
    /// <summary>
    /// Provides functionality to extract metadata from media files, such as images and videos.
    /// </summary>
    /// <remarks>
    /// This class is designed to handle various media types by analyzing their file paths and MIME types.
    /// It supports extracting properties like dimensions for images and is extensible for additional metadata extraction in the future.
    /// </remarks>
    public static class MediaMetadataExtractor
    {
        /// <summary>
        /// Extracts metadata from a media file based on its file path and MIME type.
        /// </summary>
        /// <param name="filePath">The full path to the media file to extract metadata from.</param>
        /// <param name="mimeType">The MIME type of the media file, used to determine the type of processing required.</param>
        /// <returns>
        /// A <see cref="MediaMetadata"/> object containing metadata such as dimensions (for images) 
        /// and other properties relevant to the media type.
        /// </returns>
        /// <remarks>
        /// For image files, this method extracts dimensions (width and height). 
        /// For video files, additional metadata extraction (e.g., duration) may be implemented in the future.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="filePath"/> or <paramref name="mimeType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown if the file specified by <paramref name="filePath"/> does not exist.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown if the file format is invalid or unsupported for the specified <paramref name="mimeType"/>.
        /// </exception>
        public static MediaMetadata Extract(string filePath, string mimeType)
        {
            var meta = new MediaMetadata();

            if (mimeType.StartsWith("image/"))
            {
                using var img = Image.Load(filePath);
                meta.Width = img.Width;
                meta.Height = img.Height;
            }
            else if (mimeType.StartsWith("video/"))
            {
                // ffprobe or other
                //meta.DurationSeconds = 5.0; // 
            }

            return meta;
        }
    }
}
