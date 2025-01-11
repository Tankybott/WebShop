using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Models;

namespace ControllersServices.CategoryService
{
    public class CategoryHierarchyRetriver : ICategoryHierarchyRetriver
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryHierarchyRetriver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetCategoryTreeAsync(Category parentCategory) 
        {
            IEnumerable<Category> subcategories = await GetAllSubcategoriesAsync(parentCategory);
            List<Category> categoryTree = subcategories.ToList();
            categoryTree.Add(parentCategory);
            return categoryTree;
        }

        private async Task<IEnumerable<Category>> GetAllSubcategoriesAsync(Category parentCategory)
        {
            List<Category> subcategories = new List<Category>();
            var allCategories = await _unitOfWork.Category.GetAllAsync();
            var parentSubCategories = allCategories
                .Where(c => c.ParentCategoryId == parentCategory.Id)
                .ToList();
            subcategories.AddRange(parentSubCategories);

            foreach (var subcategory in parentSubCategories)
            {
                var nestedSubcategories = await GetAllSubcategoriesAsync(subcategory);
                subcategories.AddRange(nestedSubcategories);
            }

            return subcategories;
        }
    }
}