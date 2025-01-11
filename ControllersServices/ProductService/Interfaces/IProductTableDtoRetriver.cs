using Models.DTOs;

namespace Services.ProductService.Interfaces
{
    public interface IProductTableDtoRetriver
    {
        Task<IEnumerable<ProductTableDTO>> GetProductsTableDtoAsync(int? categoryIdFilter, string? productFilterOption);
    }
}