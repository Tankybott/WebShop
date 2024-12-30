using ControllersServices.AdminCategoryService.Interfaces;
using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.AdminCategoryService
{
    public class CategoryReletedProductRemover : ICategoryReletedProductRemover
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRemover _productRemover;
        public CategoryReletedProductRemover(IProductRemover productRemover, IUnitOfWork unitOfWork)
        {
            _productRemover = productRemover;
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteProductsOfCategories(List<Category> categories)
        {
            foreach (Category cat in categories)
            {
                var productsOfCategory = await _unitOfWork.Product.GetAllAsync(p => p.CategoryId == cat.Id);
                foreach (var product in productsOfCategory)
                {
                    await _productRemover.RemoveAsync(product);
                }
            }
        }
    }
}
