using ControllersServices.AdminCategoryService.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;

namespace ControllersServices.AdminCategoryService
{
    public class AdminCategoryVMCreator : IAdminCategoryVMCreator
    {
        private readonly ICategoryHierarchyCreator _hierarchyCreator;
        public AdminCategoryVMCreator(ICategoryHierarchyCreator creator)
        {
            _hierarchyCreator = creator;
        }
        public AdminCategoryVM CreateCategoryVM(IEnumerable<Category> categories)
        {
            AdminCategoryVM categoryVM = new()
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