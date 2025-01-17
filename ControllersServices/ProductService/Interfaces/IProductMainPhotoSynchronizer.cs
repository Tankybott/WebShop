using Models.DatabaseRelatedModels;

namespace Services.ProductService.Interfaces
{
    public interface IProductMainPhotoSynchronizer
    {
        Task SynchronizeMainPhotoSetAsync(string newMainPhotoThumbnailUrl);
    }
}