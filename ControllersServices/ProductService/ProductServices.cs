
using DataAccess.Repository.IRepository;
using Models;
using Models.DTOs;
using Models.ProductModel;
using Models.ViewModels;
using Services.ProductManagement.Interfaces;



namespace ControllersServices.ProductManagement
{
    public class ProductServices : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductVMCreator _productVMCreator;
        private readonly IProductUpserter _productUpserter;
        private readonly IProductRemover _productRemover;
        private readonly IProductTableDtoRetriver _productTableDtoRetriver;

        public ProductServices(IUnitOfWork unitOfWork,
            IProductVMCreator productVMCreator,
            IProductUpserter productUpserter,
            IProductRemover productRemover,
            IProductTableDtoRetriver productTableDtoRetriver)
        {
            _unitOfWork = unitOfWork;
            _productVMCreator = productVMCreator;
            _productUpserter = productUpserter;
            _productRemover = productRemover;
            _productTableDtoRetriver = productTableDtoRetriver;
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

        public async Task<IEnumerable<ProductTableDTO>> GetProductsForTableAsync(int? categoryFilter, string? productFilterOption) 
        {
            return await _productTableDtoRetriver.GetProductsTableDtoAsync(categoryFilter, productFilterOption);
        }

        public async Task DeleteAsync(int id)
        {
            var productToDelte = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            if(productToDelte != null) await _productRemover.RemoveAsync(productToDelte);
        }
    }
}

