using Models;
using Services.CategoryService.Interfaces;


namespace Services.CategoryService
{
    public class CategoryIdRetriver : ICategoryIdRetriver
    {
        public IEnumerable<int> GetIdsOfCategories(IEnumerable<Category> categories)
        {
            var categoryFilteredIds = categories.Select(c => c.Id);
            return categoryFilteredIds;
        }
    }
}
