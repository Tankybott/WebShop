using Microsoft.AspNetCore.Http;
using Models.DatabaseRelatedModels;
using Services.PhotoService.Interfaces;
using Services.PhotoSetService.Interfaces;
using Utility.Common.Interfaces;


namespace Services.PhotoService
{
    public class PhotoService : IPhotoService
    {
        private readonly IPathCreator _pathCreator;
        private readonly IImageProcessor _imageProcessor;
        private readonly IMainPhotoSetSynchronizer _productMainPhotoSynchronizer;
        private readonly IFileService _fileService;
        public PhotoService(IPathCreator pathCreator, IImageProcessor imageProcessor, IMainPhotoSetSynchronizer productMainPhotoSynchronizer, IFileService fileService)
        {
            _pathCreator = pathCreator;
            _imageProcessor = imageProcessor;
            _productMainPhotoSynchronizer = productMainPhotoSynchronizer;
            _fileService = fileService;
        }
        public async Task UploadPhotosFromSet(IFormFile photo, string thumbnailPhotoName, string FullSizePhotName, string imageDirectory)
        {
            string roothPath = _pathCreator.GetRootPath();
            string productPath = _pathCreator.CombinePaths(roothPath, imageDirectory);
            string thumbnailPath = _pathCreator.CombinePaths(productPath, thumbnailPhotoName);
            string fullSizePath = _pathCreator.CombinePaths(productPath, FullSizePhotName);
            await _imageProcessor.CreateThumbnailAsync(photo, thumbnailPath);
            await _imageProcessor.CreateFullSizeImageAsync(photo, fullSizePath);
        }

        public async Task DeletePhotosFromServer(PhotoUrlSet photoSet)
        {
            await DeleteSinglePhotoAsync(photoSet.ThumbnailPhotoUrl);
            await DeleteSinglePhotoAsync(photoSet.BigPhotoUrl);
        }

        private async Task DeleteSinglePhotoAsync(string url)
        {
            string rootPath = _pathCreator.GetRootPath();
            string fullPath = _pathCreator.CombinePaths(rootPath, url);
            await _fileService.DeleteFileAsync(fullPath);
        }
    }
}
