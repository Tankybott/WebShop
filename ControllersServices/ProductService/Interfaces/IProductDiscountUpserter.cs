using Models;
using Models.DiscountCreateModel;

namespace Services.ProductManagement.Interfaces
{
    public interface IProductDiscountUpserter
    {
        Task HandleDiscountUpsertAsync(Product product, DiscountCreateModel discountCreateModel);
    }
}