using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository.IRepository;
using Models;

namespace ControllersServices.ProductManagement
{
    public class ProductRemover : IProductRemover
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountService _discountService;
        private readonly IPhotoService _productPhotoService;
        public ProductRemover(IUnitOfWork unitOfWork, IDiscountService discountService, IPhotoService productPhotoServices)
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
                        await _productPhotoService.DeletePhotosFromServer(urlSet);
                    }
                }
                _unitOfWork.Product.Remove(product);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
