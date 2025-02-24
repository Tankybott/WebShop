using ControllersServices.CategoryService.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;

namespace ControllersServices.CategoryService
{
    public class CategoryVMCreator : ICategoryVMCreator
    {
        private readonly ICategoryHierarchyManager _hierarchyCreator;
        public CategoryVMCreator(ICategoryHierarchyManager creator)
        {
            _hierarchyCreator = creator;
        }
        public CategoryVM CreateCategoryVM(IEnumerable<Category> categories)
        {
            CategoryVM categoryVM = new()
            {
                CategoryListItems = categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .Prepend(new SelectListItem
                {
                    Text = "Root",
                    Value = ""
                }),
                Category = new Category(),
                AllCategories = categories
            };

            return categoryVM;
        }
    }
}