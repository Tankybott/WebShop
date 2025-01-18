using Microsoft.AspNetCore.Http;

namespace Utility.Common.Interfaces
{
    public interface IImageProcessor
    {
        Task CreateFullSizeImageAsync(IFormFile inputFile, string outputFullSizePath);
        Task CreateThumbnailAsync(IFormFile inputFile, string outputThumbnailPath);
    }
}