using Models;

namespace ControllersServices.AdminCategoryService.Interfaces
{
    public interface ICategoryReletedProductRemover
    {
        Task DeleteProductsOfCategories(List<Category> categories);
    }
}