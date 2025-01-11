using Models.DTOs;
using Models.ProductModel;
using Models.ViewModels;


namespace ControllersServices.ProductManagement.Interfaces
{
    public interface IProductService
    {
        Task<ProductVM> GetProductVMForIndexAsync();
        Task<ProductVM> GetProductVMAsync(int? id = null);
        Task UpsertAsync(ProductFormModel model);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProductTableDTO>> GetProductsForTableAsync(int? CategoryFilter, string? productFilterOption);
    }
}
