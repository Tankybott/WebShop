using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.ProductManagement.Interfaces
{
    public interface IProductService
    {
        Task<ProductVM> GetProductVMForIndexAsync();
        Task<ProductVM> GetProductVMAsync(int? id = null);
        Task UpsertAsync(ProductFormModel model);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProductTableDTO>> GetProductsForTableAsync(string? CategoryFilter);
    }
}
