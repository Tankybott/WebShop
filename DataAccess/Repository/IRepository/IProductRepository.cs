using DataAccess.Repository.Utility;
using Models;
using Models.DTOs;
using Models.ProductFilterOptions;


namespace DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<ProductTableDTO>> GetProductsTableDtoAsync(IEnumerable<int>? categoryIds, string? productFilterOption);
        Task<PaginatedResult<ProductCardDTO>> GetProductCardDTOsAsync(ProductFilterOptionsQuery productFilterOptions);
    }
}
