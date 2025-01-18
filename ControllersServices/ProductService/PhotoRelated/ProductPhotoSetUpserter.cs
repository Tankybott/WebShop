using Microsoft.AspNetCore.Http;
using Models.DatabaseRelatedModels;
using Models;
using Services.ProductService.Interfaces;

namespace Services.ProductService
{

    public class ProductPhotoUpserter : IProductPhotoUpserter
    {
        private readonly IPhotoSetService _photoSetService;
        private readonly IMainPhotoSetSynchronizer _mainPhotoSetSynchronizer;

        public ProductPhotoUpserter(IPhotoSetService photoSetService, IMainPhotoSetSynchronizer photoSynchronizer)
        {
            _photoSetService = photoSetService;
            _mainPhotoSetSynchronizer = photoSynchronizer;
        }

        public async Task UploadMainPhotoSetAsync(Product product, IFormFile mainPhoto)
        {
            product.PhotosUrlSets.Add(await _photoSetService.CreatePhotoSetAsync(mainPhoto, true));
        }

        public async Task UploadOtherPhotoSetsAsync(Product product, IEnumerable<IFormFile> otherPhotos)
        {
            if (otherPhotos != null && otherPhotos.Any())
            {
                if (product.PhotosUrlSets == null)
                    product.PhotosUrlSets = new List<PhotoUrlSet>();

                foreach (var photo in otherPhotos)
                {
                    product.PhotosUrlSets.Add(await _photoSetService.CreatePhotoSetAsync(photo, false));
                }
            }
        }

        public async Task DeletePhotoSetsAsync(IEnumerable<string> thumbnailPhotoUrls)
        {
            foreach (var photoUrl in thumbnailPhotoUrls)
            {
                await _photoSetService.DeletePhotoSetAsync(photoUrl);
            }
        }

        public async Task SetPhotoMain(string newMainPhotoThumbnailUrl) 
        {
            await _mainPhotoSetSynchronizer.SynchronizeMainPhotoSetAsync(newMainPhotoThumbnailUrl);
        }

    }
}
