using Models.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.AdminCategoryService.Interfaces
{
    public interface IAdminCategoryVMCreator
    {
        AdminCategoryVM CreateCategoryVM(IEnumerable<Category> categories);
    }
}
