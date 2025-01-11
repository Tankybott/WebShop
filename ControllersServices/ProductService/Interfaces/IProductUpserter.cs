using Models.ProductModel;

namespace ControllersServices.ProductManagement.Interfaces
{
    public interface IProductUpserter
    {
        Task HandleUpsertAsync(ProductFormModel model);
    }
}