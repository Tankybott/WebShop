using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Formats.Webp;
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
            using var inputStream = inputFile.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(500, 500)
            }));

            await SaveImageAsync(image, outputThumbnailPath, 75);
        }

        public async Task CreateFullSizeImageAsync(IFormFile inputFile, string outputFullSizePath)
        {
            using var inputStream = inputFile.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(1200, 1200)
            }));

            await SaveImageAsync(image, outputFullSizePath, 75);
        }

        private async Task SaveImageAsync(Image image, string outputPath, int quality)
        {
            var encoder = new WebpEncoder
            {
                Quality = quality,
                FileFormat = WebpFileFormatType.Lossy 
            };

            await using var ms = new MemoryStream();
            await image.SaveAsync(ms, encoder);

            ms.Position = 0;
            await _fileService.CreateFileAsync(ms, outputPath);
        }
    }
}
