using Models;
using Models.DTOs;
using Models.ProductFilterOptions;
using Models.ViewModels;

namespace ControllersServices.ProductBrowserService.Interfaces
{
    public interface IProductBrowserService
    {
        Task<ProductBrowserViewModel> GetProductBrowserVM();
        Task<IEnumerable<ProductCardDTO>> GetFilteredProductsDTO(ProductFilterOptionsRequest filterOptions);
    }
}