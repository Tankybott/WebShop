using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.CategoryService.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryVM> GetCategoryVMAsync(int? id = null, int? bindedParentId = null);
        Task UpsertAsync(CategoryVM categoryVM);
        Task<IEnumerable<Category>> GetSubcategoriesOfCateogryAsync(string filterValue);
        Task DeleteCategoryWithAllSubcategoriesAsync(int? id);
    }
}
