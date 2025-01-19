using Models.DTOs;

namespace Services.ProductManagement.Interfaces
{
    public interface IProductTableDtoRetriver
    {
        Task<IEnumerable<ProductTableDTO>> GetProductsTableDtoAsync(int? categoryIdFilter, string? productFilterOption);
    }
}