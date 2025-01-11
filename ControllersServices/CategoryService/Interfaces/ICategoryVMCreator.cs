using Models.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.CategoryService.Interfaces
{
    public interface ICategoryVMCreator
    {
        CategoryVM CreateCategoryVM(IEnumerable<Category> categories);
    }
}
