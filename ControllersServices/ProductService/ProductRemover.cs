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

                if (product.PhotosUrlSets != null)
                {
                    foreach (var urlSet in product.PhotosUrlSets)
                    {
                        await _productPhotoService.DeletePhotoSetAsync(urlSet);
                    }
                }
                _unitOfWork.Product.Remove(product);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
