using AutoMapper;
using DataAccess.Repository.IRepository;
using Models;
using Models.DiscountCreateModel;
using Models.ProductModel;
using Services.PhotoService.Interfaces.DiscountService.Interfaces;
using Services.ProductManagement.Interfaces;

namespace ControllersServices.ProductManagement
{
    public class ProductUpserter : IProductUpserter
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountService _discountService;
        private readonly IProductPhotoUpserter _productPhotoUpserter;
        private readonly IProductDiscountUpserter _productDiscountUpserter;


        public ProductUpserter(IMapper mapper,
            IUnitOfWork unitOfWork,
            IDiscountService discountService,
            IProductPhotoUpserter productPhotoUpserter,
            IProductDiscountUpserter productDiscountUpserter)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _discountService = discountService;
            _productPhotoUpserter = productPhotoUpserter;
            _productDiscountUpserter = productDiscountUpserter;
        }

        public async Task HandleUpsertAsync(ProductFormModel model)
        {
            var product = new Product();
            _mapper.Map(model, product);

            await _productDiscountUpserter.HandleDiscountUpsertAsync(product, new DiscountCreateModel
            {
                StartTime = model.DiscountStartDate,
                EndTime = model.DiscountEndDate,
                Percentage = model.DiscountPercentage,
                IsDiscountChanged = model.IsDisocuntChanged,
                DiscountId = model.DiscountId
            });

            if (model.MainPhoto != null) 
            {
                await _productPhotoUpserter.UploadMainPhotoSetAsync(product, model.MainPhoto);
            }

            if (model.OtherPhotos != null) 
            {
                await _productPhotoUpserter.UploadOtherPhotoSetsAsync(product, model.OtherPhotos);
            }

            if (IsMainFlagOnIncorrectPhotoSet(product, model))
            {
                await _productPhotoUpserter.SetPhotoMain(model.MainPhotoUrl);
            }

            if (model.UrlsToDelete != null && model.UrlsToDelete.Any()) 
            {
                await _productPhotoUpserter.DeletePhotoSetsAsync(model.UrlsToDelete);
            } 

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
                if (product.Id == 0) 
                {
                    if (product.DiscountId != 0 && product.DiscountId != null)
                    {
                        await _discountService.DeleteByIdAsync(product.DiscountId.Value);
                    }
                    if (product.PhotosUrlSets != null && product.PhotosUrlSets.Any())
                    {
                        var thumbnailPhotosOfSet = product.PhotosUrlSets.Select(s => s.ThumbnailPhotoUrl);
                        await _productPhotoUpserter.DeletePhotoSetsAsync(thumbnailPhotosOfSet);
                    }
                }
                throw;
            }
        }

        private bool IsMainFlagOnIncorrectPhotoSet(Product product, ProductFormModel model)
        {
            return (product.Id != 0 && model.MainPhotoUrl != null && model.MainPhoto == null);
        }
    }
}
