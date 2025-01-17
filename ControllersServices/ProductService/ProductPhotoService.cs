using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Models.DatabaseRelatedModels;
using Services.ProductService.Interfaces;
using Utility.Common.Interfaces;


namespace ControllersServices.ProductManagement
{
    public class ProductPhotoService : IProductPhotoService
    {
        private readonly IPathCreator _pathCreator;
        private readonly IImageProcessor _imageProcessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMainPhotoSynchronizer _productMainPhotoSynchronizer;
        public ProductPhotoService(IPathCreator pathCreator, IImageProcessor imageProcessor, IUnitOfWork unitOfWork, IProductMainPhotoSynchronizer productMainPhotoSynchronizer)
        {
            _pathCreator = pathCreator;
            _imageProcessor = imageProcessor;
            _unitOfWork = unitOfWork;
            _productMainPhotoSynchronizer = productMainPhotoSynchronizer;
        }
        public async Task AddPhotoSetAsync(IFormFile photo, string thumbnailPhotoName, string FullSizePhotName, string imageDirectory)
        {
            string roothPath = _pathCreator.GetRootPath();
            string productPath = _pathCreator.CombinePaths(roothPath, imageDirectory);
            string thumbnailPath = _pathCreator.CombinePaths(productPath, thumbnailPhotoName);
            string fullSizePath = _pathCreator.CombinePaths(productPath, FullSizePhotName);
            await _imageProcessor.CreateThumbnailAsync(photo, thumbnailPath);
            await _imageProcessor.CreateFullSizeImageAsync(photo, fullSizePath);

        }

        public async Task DeletePhotoSetAsync(PhotoUrlSet photoSet)
        {
            await DeleteSinglePhotoAsync(photoSet.ThumbnailPhotoUrl);
            await DeleteSinglePhotoAsync(photoSet.BigPhotoUrl);
            _unitOfWork.PhotoUrlSets.Remove(photoSet);
            await _unitOfWork.SaveAsync();
        }

        public async Task SynchronizeMainPhotosAsync(string newPhotoThumbnailUrl)
        {
            await _productMainPhotoSynchronizer.SynchronizeMainPhotoSetAsync(newPhotoThumbnailUrl);
        }

        private async Task DeleteSinglePhotoAsync(string url) 
        {
            string rootPath = _pathCreator.GetRootPath();
            string normalizedUrl = url.Replace('/', Path.DirectorySeparatorChar);
            string fullPath = _pathCreator.CombinePaths(rootPath, normalizedUrl.TrimStart(Path.DirectorySeparatorChar));
            await _imageProcessor.DeletePhotoAsync(fullPath);
        }
    }
}
