using Models;


namespace ControllersServices.CategoryService.Interfaces
{
    public interface ICategoryHierarchyManager
    {
        Task AddSubcategoryToCategoryAsync(Category category, Category childCategory);
        Task DeleteSubcategoryFromCategoryAsync(Category parentCategory, Category childCategory);
    }
}
