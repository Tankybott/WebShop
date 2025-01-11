using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.CategoryService.Interfaces
{
    public interface ICategoryHierarchyRetriver
    {
        Task<IEnumerable<Category>> GetCategoryTreeAsync(Category parentCategory);
    }
}
