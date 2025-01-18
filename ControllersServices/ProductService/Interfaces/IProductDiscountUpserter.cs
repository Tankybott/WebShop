using Models;
using Models.DiscountCreateModel;

namespace Services.ProductService.Interfaces
{
    public interface IProductDiscountUpserter
    {
        Task HandleDiscountUpsertAsync(Product product, DiscountCreateModel discountCreateModel);
    }
}