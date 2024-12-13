using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.AdminCategoryService.Interfaces
{
    public interface ICategoryHierarchyRetriver
    {
        Task<IEnumerable<Category>> GetCollectionOfAllHigherLevelSubcategoriesAsync(Category parentCategory);
    }
}
