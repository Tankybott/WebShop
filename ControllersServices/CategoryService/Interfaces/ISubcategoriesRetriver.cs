using Models;

namespace Services.CategoryService.Interfaces
{
    public interface ISubcategoriesRetriver
    {
        Task<IEnumerable<Category>> GetSubcategoriesAsync(string parentCategoryFilter);
    }
}