using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository.IRepository;
using Models;

namespace ControllersServices.ProductManagement
{
    public class ProductRemover : IProductRemover
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountService _discountService;
        private readonly IProductPhotoService _productPhotoService;
        public ProductRemover(IUnitOfWork unitOfWork, IDiscountService discountService, IProductPhotoService productPhotoServices)
        {
            _unitOfWork = unitOfWork;
            _discountService = discountService;
            _productPhotoService = productPhotoServices;
        }
        public async Task RemoveAsync(Product product)
        {
            if (product != null)
            {
                if (product.DiscountId != null) await _discountService.DeleteByIdAsync(product.DiscountId.Value);

                await _productPhotoService.DeletePhotoAsync(product!.MainPhotoUrl);
                if (product.OtherPhotosUrls != null)
                {
                    foreach (var url in product.OtherPhotosUrls)
                    {
                        await _productPhotoService.DeletePhotoAsync(url);
                    }
                }
                _unitOfWork.Product.Remove(product);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
