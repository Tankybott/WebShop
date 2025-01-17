using ControllersServices.CategoryService.Interfaces;
using ControllersServices.ProductManagement.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;

namespace ControllersServices.CategoryService
{
    public class ProductVMCreator : IProductVMCreator
    {
        public ProductVM CreateProductVM(IEnumerable<Category> categories, IEnumerable<Product>? products)
        {
            ProductVM productVM = new()
            {
                CategoryListItems = categories.Where(c => c.SubCategories != null && !c.SubCategories.Any()).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                Product = new Product(),
                Products = new List<Product>()
            };

            if (products != null) 
            {
                productVM.Products = products.ToList();
            }

            return productVM;
        }
    }
}
