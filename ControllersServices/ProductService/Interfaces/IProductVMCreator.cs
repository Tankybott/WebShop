using Models;
using Models.ViewModels;


namespace Services.ProductManagement.Interfaces
{
    public interface IProductVMCreator
    {
        ProductVM CreateProductVM(IEnumerable<Category> categories, IEnumerable<Product>? products = null);
    }
}
