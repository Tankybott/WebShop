using Models;
using Models.DTOs;
using Models.ProductFilterOptions;


namespace DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<ProductTableDTO>> GetProductsTableDtoAsync(IEnumerable<int>? categoryIds, string? productFilterOption);
        Task<IEnumerable<ProductCardDTO>> GetProductCardDTOsAsync(ProductFilterOptionsQuery productFilterOptions);
    }
}
