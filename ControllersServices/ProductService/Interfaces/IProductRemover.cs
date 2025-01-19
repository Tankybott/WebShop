using Models;

namespace Services.ProductManagement.Interfaces
{
    public interface IProductRemover
    {
        Task RemoveAsync(Product product);
    }
}