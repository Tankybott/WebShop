using ControllersServices.AdminCategoryService.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Models;

namespace ControllersServices.AdminCategoryService
{
    public class CategoryHierarchyRetriver : ICategoryHierarchyRetriver
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryHierarchyRetriver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetCollectionOfAllHigherLevelSubcategoriesAsync(Category parentCategory)
        {
            List<Category> subcategories = new List<Category>();
            var allCategories = await _unitOfWork.Category.GetAllAsync();
            var parentSubCategories = allCategories
                .Where(c => c.ParentCategoryId == parentCategory.Id)
                .ToList();
            subcategories.AddRange(parentSubCategories);

            foreach (var subcategory in parentSubCategories)
            {
                var nestedSubcategories = await GetCollectionOfAllHigherLevelSubcategoriesAsync(subcategory);
                subcategories.AddRange(nestedSubcategories);
            }

            return subcategories;
        }
    }
}