using Models.FormModel;

namespace Services.ProductManagement.Interfaces
{
    public interface IProductUpserter
    {
        Task HandleUpsertAsync(ProductFormModel model);
    }
}