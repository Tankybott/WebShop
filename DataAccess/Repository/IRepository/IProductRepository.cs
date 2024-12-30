using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<ProductTableDTO>> GetProductsDtoAsync();
        Task<IEnumerable<ProductTableDTO>> GetProductsDtoOfCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductOfCategoryAsync(int categoryId);

    }
}
