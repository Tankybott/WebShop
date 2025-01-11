using AutoMapper;
using ControllersServices.ProductManagement.Interfaces;
using ControllersServices.Utilities.Interfaces;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Models.ProductModel;
using Serilog;





namespace ControllersServices.ProductManagement
{
    public class ProductUpserter : IProductUpserter
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductPhotoService _productPhotoService;
        private readonly IFileNameCreator _fileNameCreator;
        private readonly IDiscountService _discountService;

        private const string _productImageDirectory = "images/product";

        public ProductUpserter(IMapper mapper,
            IUnitOfWork unitOfWork,
            IFileNameCreator fileNameCreator,
            IProductPhotoService productPhotoService,
            IDiscountService discountService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileNameCreator = fileNameCreator;
            _productPhotoService = productPhotoService;
            _discountService = discountService;
        }

        public async Task HandleUpsertAsync(ProductFormModel model)
        {
            var product = new Product();
            _mapper.Map(model, product);

            await HandleDiscountUpsert(product, model.DiscountStartDate, model.DiscountEndDate, model.DiscountPercentage, model.IsDisocuntChanged, model.DiscountId);

            await HandleMainPhotoUpload(product, model.MainPhoto);

            await HandleOtherPhotosUpload(product, model.OtherPhotos);

            await HandlePhotosDeletion(model.UrlsToDelete);

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
                //delete created discount if there was a problem with adding whole product
                if (product.DiscountId != 0 && product.DiscountId != null)
                {
                    var discountToDelete = await _unitOfWork.Discount.GetAsync(d => d.Id == product.DiscountId);
                    _unitOfWork.Discount.Remove(discountToDelete);
                }
                throw;
            }

        }

        private async Task HandleDiscountUpsert(
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

        private async Task HandleMainPhotoUpload(Product product, IFormFile mainPhoto)
        {
            if (mainPhoto != null)
            {
                string fileName = _fileNameCreator.CreateFileName(mainPhoto);
                await _productPhotoService.AddPhotoAsync(mainPhoto, fileName, _productImageDirectory);
                product.MainPhotoUrl = $"/{_productImageDirectory}/{fileName}";
            }
        }

        private async Task HandleOtherPhotosUpload(Product product, IEnumerable<IFormFile> otherPhotos)
        {
            if (otherPhotos != null && otherPhotos.Any())
            {
                if (product.OtherPhotosUrls == null)
                    product.OtherPhotosUrls = new List<string>();

                foreach (var photo in otherPhotos)
                {
                    string fileName = _fileNameCreator.CreateFileName(photo);
                    await _productPhotoService.AddPhotoAsync(photo, fileName, _productImageDirectory);
                    product.OtherPhotosUrls.Add($"/{_productImageDirectory}/{fileName}");
                }
            }
        }

        private async Task HandlePhotosDeletion(IEnumerable<string> urlsToDelete)
        {
            if (urlsToDelete != null)
            {
                foreach (var url in urlsToDelete)
                {
                    await _productPhotoService.DeletePhotoAsync(url);
                }
            }
        }
    }
}
