using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository.IRepository;
using Models;
using Services.ProductManagement.Interfaces;


namespace ControllersServices.CategoryService
{
    public class CategoryReletedProductRemover : ICategoryReletedProductRemover
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRemover _productRemover;
        public CategoryReletedProductRemover(IProductRemover productRemover, IUnitOfWork unitOfWork)
        {
            _productRemover = productRemover;
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteProductsOfCategoriesAsync(IEnumerable<Category> categories)
        {
            foreach (Category cat in categories)
            {
                var productsOfCategory = await _unitOfWork.Product.GetAllAsync(p => p.CategoryId == cat.Id);
                foreach (var product in productsOfCategory)
                {
                    await _productRemover.RemoveAsync(product);
                }
            }
        }
    }
}
