using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.AdminCategoryService.Interfaces
{
    public interface IAdminCategoryService
    {
        Task<AdminCategoryVM> GetAdminCategoryVMAsync(int? id = null, int? bindedParentId = null);
        Task UpsertAsync(AdminCategoryVM categoryVM);
        Task<IEnumerable<Category>> GetSubcategoriesOfCateogryAsync(string filterValue);
        Task DeleteCategoryWithWholeTreeOfSubcategories(int? id);
    }
}
