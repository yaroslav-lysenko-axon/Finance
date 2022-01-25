using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace File.Application.Extensions
{
    public static class ImagesExtensions
    {
        public static async Task<byte[]> GetFileBytesAfterResizing(Image image, int uploadSize, CancellationToken cancellationToken)
        {
            await using var outStream = new MemoryStream();
            var (width, height) = CalculateResizedImageDimensions(image.Width, image.Height, uploadSize);
            image.Mutate(operation => operation.Resize(width, height));
            await image.SaveAsync(outStream, PngFormat.Instance, cancellationToken);
            await outStream.FlushAsync(cancellationToken);
            var fileBytes = outStream.ToArray();
            return fileBytes;
        }

        public static (int Width, int Height) CalculateResizedImageDimensions(int imageWidth, int imageHeight, int resizeTo)
        {
            if (imageWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(imageWidth), "Image width should be positive");
            }

            if (imageHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(imageHeight), "Image height should be positive");
            }

            var maxSize = Math.Max(imageWidth, imageHeight);

            // check if resizing is needed
            if (maxSize == resizeTo)
            {
                return (imageWidth, imageHeight);
            }

            var scaleFactor = 1d / maxSize;

            var newWidth = (int) Math.Round(imageWidth * scaleFactor * resizeTo);
            var newHeight = (int) Math.Round(imageHeight * scaleFactor * resizeTo);

            return (newWidth, newHeight);
        }
    }
}
