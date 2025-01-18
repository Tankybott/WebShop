using Models.DatabaseRelatedModels;

namespace Services.ProductService.Interfaces
{
    public interface IMainPhotoSetSynchronizer
    {
        Task SynchronizeMainPhotoSetAsync(string newMainPhotoThumbnailUrl);
    }
}