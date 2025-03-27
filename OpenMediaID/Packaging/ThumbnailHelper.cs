using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace OpenMediaID.Packaging;

/// <summary>
///     Provides utility methods for creating and processing thumbnails from images.
/// </summary>
/// <remarks>
///     This class is designed to assist with resizing images to generate thumbnails.
///     It utilizes the ImageSharp library for image processing.
/// </remarks>
public static class ThumbnailHelper
{
    /// <summary>
    ///     Creates a thumbnail from the specified input image file and saves it to the specified output path.
    /// </summary>
    /// <param name="inputPath">The file path of the input image to be processed.</param>
    /// <param name="outputPath">The file path where the generated thumbnail will be saved.</param>
    /// <param name="maxSize">
    ///     The maximum size (in pixels) for the width or height of the thumbnail.
    ///     The aspect ratio of the original image is preserved. Default is 256.
    /// </param>
    /// <remarks>
    ///     This method uses the ImageSharp library to resize the input image while maintaining its aspect ratio.
    ///     The thumbnail is resized to fit within a square of the specified maximum size.
    /// </remarks>
    /// <exception cref="System.IO.FileNotFoundException">
    ///     Thrown if the file specified by <paramref name="inputPath" /> does not exist.
    /// </exception>
    /// <exception cref="SixLabors.ImageSharp.UnknownImageFormatException">
    ///     Thrown if the input file format is not supported by ImageSharp.
    /// </exception>
    /// <exception cref="System.UnauthorizedAccessException">
    ///     Thrown if the application does not have permission to read the input file or write to the output path.
    /// </exception>
    public static void CreateThumbnail(string inputPath, string outputPath, int maxSize = 256)
    {
        using var image = Image.Load(inputPath);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(maxSize, maxSize)
        }));
        image.Save(outputPath);
    }
}