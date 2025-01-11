using Models;

namespace ControllersServices.CategoryService.Interfaces
{
    public interface ICategoryReletedProductRemover
    {
        Task DeleteProductsOfCategoriesAsync(IEnumerable<Category> categories);
    }
}