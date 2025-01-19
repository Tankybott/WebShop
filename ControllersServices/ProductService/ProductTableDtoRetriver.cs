using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository.IRepository;
using Models;
using Models.DTOs;
using Services.CategoryService.Interfaces;
using Services.ProductManagement.Interfaces;


namespace Services.ProductManagement
{
    public class ProductTableDtoRetriver : IProductTableDtoRetriver
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryIdRetriver _categoryIdRetriver;
        private readonly ICategoryHierarchyRetriver _categoryHierarchyRetriver;

        public ProductTableDtoRetriver(IUnitOfWork unitOfWork, ICategoryIdRetriver categoryIdRetriver, ICategoryHierarchyRetriver categoryHierarchyRetriver)
        {
            _unitOfWork = unitOfWork;
            _categoryIdRetriver = categoryIdRetriver;
            _categoryHierarchyRetriver = categoryHierarchyRetriver;
        }

        public async Task<IEnumerable<ProductTableDTO>> GetProductsTableDtoAsync(int? categoryIdFilter, string? productFilterOption)
        {
            var parentCategory = await _unitOfWork.Category.GetAsync(c => c.Id == categoryIdFilter);
            IEnumerable<int> categoriesTreeIds = [];

            if (categoryIdFilter != null)
            {
                if (parentCategory != null) categoriesTreeIds = _categoryIdRetriver.GetIdsOfCategories(await _categoryHierarchyRetriver.GetCategoryTreeAsync(parentCategory));
            }

            var products = await _unitOfWork.Product.GetProductsTableDtoAsync(categoriesTreeIds, productFilterOption);
            return products;
        }
    }
}
