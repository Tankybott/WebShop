using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.ProductService.Interfaces
{
    public interface IProductService
    {
        // add bootsrap icons 
        Task<ProductVM> GetProductVMForIndexAsync();
        Task<ProductVM> GetProductVMAsync(int? id = null);
        Task UpsertAsync(ProductFormModel model);
        Task Delete(int? id);
        Task<IEnumerable<ProductDTO>> GetProductsForTableAsync(string? CategoryFilter);
    }
}
