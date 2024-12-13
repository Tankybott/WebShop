using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.AdminCategoryService.Interfaces
{
    public interface ICategoryHierarchyCreator
    {
        Task AddSubcategoryToCategoryAsync(Category category, Category childCategory);
        Task DeleteSubcategoryFromCategoryAsync(Category parentCategory, Category childCategory);
    }
}
