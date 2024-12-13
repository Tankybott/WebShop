using AutoMapper;
using ControllersServices.ProductService.Interfaces;
using ControllersServices.Utilities.Interfaces;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.CustomExceptions;

namespace ControllersServices.ProductService
{
    public class ProductUpserter : IProductUpserter
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductPhotoService _productPhotoService;
        private readonly IFileNameCreator _fileNameCreator;
        private readonly IDiscountService _discountService;

        private const string _productImageDirectory = "images/product";

        public ProductUpserter(IMapper mapper, IUnitOfWork unitOfWork, IFileNameCreator fileNameCreator, IProductPhotoService productPhotoService, IDiscountService discountService)
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

            try
            {
                await HandleDiscountUpsert(product, model.DiscountStartDate, model.DiscountEndDate, model.DiscountPercentage);
            }
            catch (DiscountUpsertException)
            {
                throw; 
            }

            await HandleMainPhotoUpload(product, model.MainPhoto);

            await HandleOtherPhotosUpload(product, model.OtherPhotos);

            await HandlePhotosDeletion(model.OtherPhotosUrls);

            if (product.Id == 0)
            {
                _unitOfWork.Product.Add(product);
            }
            else
            {
                _unitOfWork.Product.Update(product);
            }


            await _unitOfWork.SaveAsync();
        }

        private async Task HandleDiscountUpsert(Product product, DateTime? startTime, DateTime? endTime, int? percentage)
        {
            if (startTime != null && endTime != null && percentage != null)
            {
                if (product.DiscountId == 0 || product.DiscountId == null)
                {
                    var discount = await _discountService.CreateDiscountAsync(startTime.Value, endTime.Value, percentage.Value);
                    product.DiscountId = discount.Id;
                } else
                {
                    product.Discount.StartTime = startTime.Value;
                    product.Discount.EndTime = endTime.Value;
                    product.Discount.Percentage = percentage.Value;
                }
            } else if (startTime != null || endTime != null || percentage != null ) 
            {
                Log.Error("Discount not added beacuse of missing data");
                throw new DiscountUpsertException("Discount was not added because of missing or invalid data.");
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
