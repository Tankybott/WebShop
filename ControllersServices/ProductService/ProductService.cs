
using ControllersServices.ProductManagement.Interfaces;

using DataAccess.Repository.IRepository;

using Models;
using Models.ViewModels;


namespace ControllersServices.ProductManagement
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductVMCreator _productVMCreator;
        private readonly IProductUpserter _productUpserter;
        private readonly IProductRemover _productRemover;

        public ProductService(IUnitOfWork unitOfWork,
            IProductVMCreator productVMCreator,
            IProductUpserter productUpserter,
            IProductRemover productRemover)
        {
            _unitOfWork = unitOfWork;
            _productVMCreator = productVMCreator;
            _productUpserter = productUpserter;
            _productRemover = productRemover;
        }

        public async Task<ProductVM> GetProductVMForIndexAsync()
        {
            var categories = await _unitOfWork.Category.GetAllAsync(tracked: true, sortBy: c => c.Name);
            var products = await _unitOfWork.Product.GetAllAsync(tracked: true);

            ProductVM producVM = _productVMCreator.CreateProductVM(categories, products);

            return producVM;
        }

        public async Task<ProductVM> GetProductVMAsync(int? id = null)
        {
            var categories = await _unitOfWork.Category.GetAllAsync(tracked: true, sortBy: c => c.Name);

            ProductVM producVM = _productVMCreator.CreateProductVM(categories);

            if (id != null) 
            {
               producVM.Product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            }

            return producVM;
        }

        public async Task UpsertAsync(ProductFormModel model)
        {
            await _productUpserter.HandleUpsertAsync(model);
        }

        public async Task<IEnumerable<ProductTableDTO>> GetProductsForTableAsync(string? categoryFilter) 
        {
            int filterValueInt;
            if (categoryFilter != null && int.TryParse(categoryFilter, out filterValueInt))
            {
                return await _unitOfWork.Product.GetProductsDtoOfCategoryAsync(filterValueInt);
            }

            var products = await _unitOfWork.Product.GetProductsDtoAsync();
            return products;
        }

        public async Task DeleteAsync(int id)
        {
            var productToDelte = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            if(productToDelte != null) await _productRemover.RemoveAsync(productToDelte);
        }
    }
}

