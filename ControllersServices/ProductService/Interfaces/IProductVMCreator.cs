using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.ProductService.Interfaces
{
    public interface IProductVMCreator
    {
        ProductVM CreateProductVM(IEnumerable<Category> categories, IEnumerable<Product>? products = null);
    }
}
