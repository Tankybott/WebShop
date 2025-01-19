using Models.DatabaseRelatedModels;

namespace Services.PhotoSetService.Interfaces
{
    public interface IMainPhotoSetSynchronizer
    {
        Task SynchronizeMainPhotoSetAsync(string newMainPhotoThumbnailUrl);
    }
}