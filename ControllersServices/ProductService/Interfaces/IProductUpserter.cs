using Models;

namespace ControllersServices.ProductManagement.Interfaces
{
    public interface IProductUpserter
    {
        Task HandleUpsertAsync(ProductFormModel model);
    }
}