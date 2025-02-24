namespace Services.ProductService.Interfaces
{
    public interface IProductPriceRetriver
    {
        Task<decimal> GetProductPriceAsync(int productId);
    }
}