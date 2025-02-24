using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository.IRepository;
using Models;
using Services.CategoryService.Interfaces;

namespace Services.CategoryService
{
    public class CategoryRemover : ICategoryRemover
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryHierarchyManager _categoryHierarchyManager;
        private readonly ICategoryHierarchyRetriver _categoryHierarchyRetriver;
        private readonly ICategoryReletedProductRemover _productRemover;

        public CategoryRemover(IUnitOfWork unitOfWork, ICategoryReletedProductRemover productRemover, ICategoryHierarchyManager categoryHierarchyManager, ICategoryHierarchyRetriver categoryHierarchyRetriver)
        {
            _unitOfWork = unitOfWork;
            _productRemover = productRemover;
            _categoryHierarchyManager = categoryHierarchyManager;
            _categoryHierarchyRetriver = categoryHierarchyRetriver;
        }

        public async Task DeleteAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var targetCategory = await _unitOfWork.Category.GetAsync(u => u.Id == id, includeProperties: "ParentCategory");
                if (targetCategory != null)
                {
                    if (targetCategory.ParentCategoryId != null && targetCategory.ParentCategoryId != 0)
                    {
                        await _categoryHierarchyManager.DeleteSubcategoryFromCategoryAsync(targetCategory, targetCategory.ParentCategory!);
                    }
                    IEnumerable<Category> categoriesToBeDeleted = await _categoryHierarchyRetriver.GetCategoryTreeAsync(targetCategory);

                    await _productRemover.DeleteProductsOfCategoriesAsync(categoriesToBeDeleted);
                    _unitOfWork.Category.RemoveRange(categoriesToBeDeleted);
                    await _unitOfWork.SaveAsync();
                }
            }
        }
    }
}
