using Models;

namespace Services.CategoryService.Interfaces
{
    public interface ICategoryIdRetriver
    {
        IEnumerable<int> GetIdsOfCategories(IEnumerable<Category> categories);
    }
}