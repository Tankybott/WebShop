using Models;

namespace ControllersServices.ProductService.Interfaces
{
    public interface IProductUpserter
    {
        Task HandleUpsertAsync(ProductFormModel model);
    }
}