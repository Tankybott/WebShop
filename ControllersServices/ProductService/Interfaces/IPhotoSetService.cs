using Microsoft.AspNetCore.Http;
using Models;
using Models.DatabaseRelatedModels;

namespace Services.ProductService.Interfaces
{
    public interface IPhotoSetService
    {
        Task<PhotoUrlSet> CreatePhotoSetAsync(IFormFile photo, bool isMain);
        Task DeletePhotoSetAsync(string thumbnailPhotoUrl);
    }
}