using Models.ProductModel;

namespace Services.ProductManagement.Interfaces
{
    public interface IProductUpserter
    {
        Task HandleUpsertAsync(ProductFormModel model);
    }
}