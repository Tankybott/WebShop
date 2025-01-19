using Microsoft.AspNetCore.Http;
using Models;

namespace Services.ProductManagement.Interfaces
{
    public interface IProductPhotoUpserter
    {
        Task DeletePhotoSetsAsync(IEnumerable<string> thumbnailPhotoUrls);
        Task UploadMainPhotoSetAsync(Product product, IFormFile mainPhoto);
        Task UploadOtherPhotoSetsAsync(Product product, IEnumerable<IFormFile> otherPhotos);
        Task SetPhotoMain(string newMainPhotoThumbnailUrl);
    }
}