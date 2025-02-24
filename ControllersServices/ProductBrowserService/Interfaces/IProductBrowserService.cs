using DataAccess.Repository.Utility;
using Models;
using Models.DTOs;
using Models.ProductFilterOptions;
using Models.ViewModels;

namespace ControllersServices.ProductBrowserService.Interfaces
{
    public interface IProductBrowserService
    {
        Task<ProductBrowserVM> GetProductBrowserVM();
        Task<PaginatedResult<ProductCardDTO>> GetFilteredProductsDTO(ProductFilterOptionsRequest filterOptions);
    }
}