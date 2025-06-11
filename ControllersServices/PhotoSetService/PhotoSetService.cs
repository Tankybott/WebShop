using Microsoft.AspNetCore.Http;
using Models.DatabaseRelatedModels;
using Utility.Common.Interfaces;
using DataAccess.Repository.IRepository;
using Services.PhotoService.Interfaces;
using Services.PhotoSetService.Interfaces;

namespace Services.PhotoSetService
{
    public class PhotoSetService : IPhotoSetService
    {
        private readonly IFileNameCreator _fileNameCreator;
        private readonly IPhotoService _productPhotoService;
        private readonly IPathCreator _pathCreator;
        private readonly IUnitOfWork _unitOfWork;

        private const string ProductImageDirectory = "images/product";
        public PhotoSetService(IFileNameCreator fileNameCreator, IPhotoService productPhotoService, IPathCreator pathCreator, IUnitOfWork unitOfWork)
        {
            _fileNameCreator = fileNameCreator;
            _productPhotoService = productPhotoService;
            _pathCreator = pathCreator;
            _unitOfWork = unitOfWork;
        }

        public async Task<PhotoUrlSet> CreatePhotoSetAsync(IFormFile photo, bool isMain)
        {
            string thumbnailPhotoFileName = _fileNameCreator.CreateFileName("webp");
            string fullSizePhotoFileName = _fileNameCreator.CreateFileName("webp");
            await _productPhotoService.UploadPhotosFromSet(photo, thumbnailPhotoFileName, fullSizePhotoFileName, ProductImageDirectory);
            return new PhotoUrlSet
            {
                ThumbnailPhotoUrl = _pathCreator.CreateUrlPath(ProductImageDirectory, thumbnailPhotoFileName),
                BigPhotoUrl = _pathCreator.CreateUrlPath(ProductImageDirectory, fullSizePhotoFileName),
                IsMainPhoto = isMain
            };
        }

        public async Task DeletePhotoSetAsync(string thumbnailPhotoUrl)
        {
            var photoUrlSet = await _unitOfWork.PhotoUrlSets.GetAsync(p => p.ThumbnailPhotoUrl == thumbnailPhotoUrl);
            if (photoUrlSet != null)
            {
                await _productPhotoService.DeletePhotosFromServer(photoUrlSet);
                _unitOfWork.PhotoUrlSets.Remove(photoUrlSet);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
