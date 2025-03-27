using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OpenMediaID.Packaging
{
    public static class ThumbnailHelper
    {
        public static void CreateImageThumbnail(string inputPath, string outputPath, int maxSize = 256)
        {
            using var image = Image.FromFile(inputPath);

            int width = image.Width;
            int height = image.Height;

            float scale = Math.Min((float)maxSize / width, (float)maxSize / height);
            int thumbWidth = (int)(width * scale);
            int thumbHeight = (int)(height * scale);

            using var thumb = new Bitmap(image, new Size(thumbWidth, thumbHeight));
            thumb.Save(outputPath, ImageFormat.Jpeg);
        }
    }
}