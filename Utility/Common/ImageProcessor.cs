using Microsoft.AspNetCore.Http;
using SkiaSharp;
using Utility.Common.Interfaces;

namespace Utility.Common
{
    public class ImageProcessor : IImageProcessor
    {
        private readonly IFileService _fileService;

        public ImageProcessor(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task CreateThumbnailAsync(IFormFile inputFile, string outputThumbnailPath)
        {
            await using var inputStream = inputFile.OpenReadStream();
            using var original = SKBitmap.Decode(inputStream);

            var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

            var thumbnail = ResizeToFit(original, 150, 150, samplingOptions);
            if (thumbnail != null)
            {
                await SaveImageAsync(thumbnail, outputThumbnailPath, 90);
            }
        }

        public async Task CreateFullSizeImageAsync(IFormFile inputFile, string outputFullSizePath)
        {
            await using var inputStream = inputFile.OpenReadStream();
            using var original = SKBitmap.Decode(inputStream);

            var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

            var fullSize = ResizeToFit(original, 1200, 800, samplingOptions);
            if (fullSize != null)
            {
                await SaveImageAsync(fullSize, outputFullSizePath, 90);
            }
        }

        private SKBitmap ResizeToFit(SKBitmap original, int maxWidth, int maxHeight, SKSamplingOptions samplingOptions)
        {
            float widthRatio = (float)maxWidth / original.Width;
            float heightRatio = (float)maxHeight / original.Height;
            float scale = Math.Min(widthRatio, heightRatio);

            int newWidth = (int)(original.Width * scale);
            int newHeight = (int)(original.Height * scale);

            return original.Resize(new SKImageInfo(newWidth, newHeight), samplingOptions);
        }

        private async Task SaveImageAsync(SKBitmap bitmap, string outputPath, int quality)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var memoryStream = new MemoryStream();
            image.Encode(SKEncodedImageFormat.Jpeg, quality).SaveTo(memoryStream);

            memoryStream.Position = 0;
            await _fileService.CreateFileAsync(memoryStream, outputPath);
        }
    }
}