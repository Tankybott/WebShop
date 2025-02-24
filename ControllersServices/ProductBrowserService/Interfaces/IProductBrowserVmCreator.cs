using Models.ViewModels;

namespace ControllersServices.ProductBrowserService.Interfaces
{
    public interface IProductBrowserVmCreator
    {
        Task<ProductBrowserVM> CreateProductBrowserVM();
    }
}