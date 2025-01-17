using AutoMapper;
using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Models.DatabaseRelatedModels;
using Models.ProductModel;
using Serilog;
using Utility.Common.Interfaces;

namespace ControllersServices.ProductManagement
{
    public class ProductUpserter : IProductUpserter
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductPhotoService _productPhotoService;
        private readonly IFileNameCreator _fileNameCreator;
        private readonly IDiscountService _discountService;
        private readonly IPathCreator _pathCreator;

        private const string _productImageDirectory = "images/product";

        public ProductUpserter(IMapper mapper,
            IUnitOfWork unitOfWork,
            IFileNameCreator fileNameCreator,
            IProductPhotoService productPhotoService,
            IDiscountService discountService,
            IPathCreator pathCreator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileNameCreator = fileNameCreator;
            _productPhotoService = productPhotoService;
            _discountService = discountService;
            _pathCreator = pathCreator;
        }

        public async Task HandleUpsertAsync(ProductFormModel model)
        {
            var product = new Product();
            _mapper.Map(model, product);

            await HandleDiscountUpsertAsync(product, model.DiscountStartDate, model.DiscountEndDate, model.DiscountPercentage, model.IsDisocuntChanged, model.DiscountId);

            await HandleMainPhotoUploadAsync(product, model.MainPhoto);

            await HandleOtherPhotosUploadAsync(product, model.OtherPhotos);

            //When there is possibility that main photo was swapped with any other photos belonging to product 
            if (product.Id != 0 && model.MainPhotoUrl != null && model.MainPhoto == null)
            {
                await _productPhotoService.SynchronizeMainPhotosAsync(model.MainPhotoUrl);
            }

            if (model.UrlsToDelete != null && model.UrlsToDelete.Any()) await HandlePhotoSetsDeletionAsync(model.UrlsToDelete);

            if (product.Id == 0)
            {
                _unitOfWork.Product.Add(product);
            }
            else
            {
                _unitOfWork.Product.Update(product);
            }

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                if (product.DiscountId != 0 && product.DiscountId != null)
                {
                    var discountToDelete = await _unitOfWork.Discount.GetAsync(d => d.Id == product.DiscountId);
                    if (discountToDelete != null) 
                    {
                        _unitOfWork.Discount.Remove(discountToDelete); 
                    }
                }
                throw;
            }

        }

        private async Task HandleDiscountUpsertAsync(
            Product product,
            DateTime? startTime,
            DateTime? endTime,
            int? percentage,
            bool? isDiscountChanged,
            int? discountId)
        {
            if (startTime != null && endTime != null && percentage != null)
            {
                if (isDiscountChanged != null && isDiscountChanged == false) 
                {
                    return;
                }
                else if (product.DiscountId == 0 || product.DiscountId == null)
                {
                    var discount = await _discountService.CreateDiscountAsync(startTime.Value, endTime.Value, percentage.Value);
                    product.DiscountId = discount.Id;
                }
                else
                {
                    var discount = await _discountService.UpdateDiscountAsync(product.DiscountId.Value, startTime.Value, endTime.Value, percentage.Value);
                    product.DiscountId = discount.Id;
                }
            } 
            else 
            {
                if (startTime == null && endTime == null && percentage == null)
                {
                    if (discountId != 0 && discountId != null)
                    {
                        await _discountService.DeleteByIdAsync(discountId.Value);
                    }
                    product.DiscountId = null;
                }
                else 
                {
                    Log.Error("Failed to add discount because of invalid data");
                    throw new ArgumentException("Failed to add discount because of invalid data ");
                }
            }
        }

        private async Task HandleMainPhotoUploadAsync(Product product, IFormFile mainPhoto)
        {
            if (mainPhoto != null)
            {
                await HandleAddingPhoto(product, mainPhoto, true);
            }
        }

        private async Task HandleOtherPhotosUploadAsync(Product product, IEnumerable<IFormFile> otherPhotos)
        {
            if (otherPhotos != null && otherPhotos.Any())
            {
                if (product.PhotosUrlSets == null)
                    product.PhotosUrlSets = new List<PhotoUrlSet>();

                foreach (var photo in otherPhotos)
                {
                    await HandleAddingPhoto(product, photo, false);
                }
            }
        }

        private async Task HandleAddingPhoto(Product product, IFormFile photo, bool isMain)
        {
            string thumbnailPhotoFileName = _fileNameCreator.CreateJpegFileName();
            string fullSizePhotoFileName = _fileNameCreator.CreateJpegFileName();
            await _productPhotoService.AddPhotoSetAsync(photo, thumbnailPhotoFileName, fullSizePhotoFileName, _productImageDirectory);
            var photoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = _pathCreator.CreateUrlPath(_productImageDirectory, thumbnailPhotoFileName),
                BigPhotoUrl = _pathCreator.CreateUrlPath(_productImageDirectory, fullSizePhotoFileName),
                IsMainPhoto = isMain
            };
            _unitOfWork.PhotoUrlSets.Add(photoSet);
            await _unitOfWork.SaveAsync();
            product?.PhotosUrlSets?.Add(photoSet);
        }

        private async Task HandlePhotoSetsDeletionAsync(IEnumerable<string> thumbnailPhotoUrls)
        {
            var photoSetsToDelete = new List<PhotoUrlSet>();
            foreach (var photoUrl in thumbnailPhotoUrls) 
            {
                var photoUrlSet = await _unitOfWork.PhotoUrlSets.GetAsync(p => p.ThumbnailPhotoUrl == photoUrl);
                if (photoUrlSet != null) photoSetsToDelete.Add(photoUrlSet);
            }

            if (photoSetsToDelete != null)
            {
                foreach (var urlSet in photoSetsToDelete)
                {
                    await _productPhotoService.DeletePhotoSetAsync(urlSet);
                }
            }
        }
    }
}
