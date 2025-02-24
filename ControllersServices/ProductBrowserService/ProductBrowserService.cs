using AutoMapper;
using ControllersServices.CategoryService.Interfaces;
using ControllersServices.ProductBrowserService.Interfaces;
using DataAccess.Repository.IRepository;
using DataAccess.Repository.Utility;
using Models;
using Models.DTOs;
using Models.ProductFilterOptions;
using Models.ViewModels;
using Services.CategoryService.Interfaces;


namespace ControllersServices.ProductBrowserService
{
    public class ProductBrowserService : IProductBrowserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductBrowserVmCreator _vmCreator;
        private readonly ICategoryHierarchyRetriver _categoryHierarchyRetriver;
        private readonly ICategoryIdRetriver _categoryIdRetriver;
        private readonly IMapper _mapper;

        public ProductBrowserService(IUnitOfWork unitOfWork,
            IProductBrowserVmCreator vmCreator,
            ICategoryHierarchyRetriver categoryHierarchyRetriver,
            IMapper mapper,
            ICategoryIdRetriver categoryIdRetriver)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = vmCreator;
            _categoryHierarchyRetriver = categoryHierarchyRetriver;
            _mapper = mapper;
            _categoryIdRetriver = categoryIdRetriver;
        }

        public async Task<ProductBrowserVM> GetProductBrowserVM()
        {
            return await _vmCreator.CreateProductBrowserVM();
        }

        public async Task<PaginatedResult<ProductCardDTO>> GetFilteredProductsDTO(ProductFilterOptionsRequest filterOptions)
        {
            var productCategory = await _unitOfWork.Category.GetAsync(c => c.Id == filterOptions.CategoryIDFilter);
            IEnumerable<int> categoriesTreeIds = [];

            if (productCategory != null) 
            {
                categoriesTreeIds = _categoryIdRetriver.GetIdsOfCategories(await _categoryHierarchyRetriver.GetCategoryTreeAsync(productCategory));
            }

            return await _unitOfWork.Product.GetProductCardDTOsAsync(MapFilterOptions(filterOptions, categoriesTreeIds));
        }

        private ProductFilterOptionsQuery MapFilterOptions(ProductFilterOptionsRequest filterOptions, IEnumerable<int> categoriesIds) 
        {
            var productFilterOptionQuery = new ProductFilterOptionsQuery();
            _mapper.Map(filterOptions, productFilterOptionQuery);
            productFilterOptionQuery.CategoriesFilteredIds = categoriesIds;
            return productFilterOptionQuery;
        }
    }
}
