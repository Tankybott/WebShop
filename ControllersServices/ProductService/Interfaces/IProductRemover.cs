using Models;

namespace ControllersServices.ProductManagement.Interfaces
{
    public interface IProductRemover
    {
        Task RemoveAsync(Product product);
    }
}