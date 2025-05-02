
using Models.ViewModels;
using Models;
using DataAccess.Repository.IRepository;
using ControllersServices.CategoryService.Interfaces;
using Services.CategoryService.Interfaces;

namespace Services.CategoryService
{
    public class CategoryUpserter : ICategoryUpserter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryHierarchyManager _categoryHierarchyCreator;

        public CategoryUpserter(IUnitOfWork unitOfWork, ICategoryHierarchyManager categoryHierarchyCreator)
        {
            _unitOfWork = unitOfWork;
            _categoryHierarchyCreator = categoryHierarchyCreator;
        }

        public async Task UpsertAsync(CategoryVM categoryVM)
        {
            if (categoryVM.Category.Id == 0 )
            {
                _unitOfWork.Category.Add(categoryVM.Category);
                await _unitOfWork.SaveAsync();
                await BindParentCategoryAsync(categoryVM.Category);
            }
            else
            {
                _unitOfWork.Category.Update(categoryVM.Category);
                await _unitOfWork.SaveAsync();
            }
        }

        private async Task BindParentCategoryAsync(Category category)
        {
            if (category.ParentCategoryId != null)
            {
                var parentCategory = await _unitOfWork.Category.GetAsync(c => c.Id == category.ParentCategoryId);
                if (parentCategory != null) await _categoryHierarchyCreator.AddSubcategoryToCategoryAsync(parentCategory, category);
            }
        }
    }
}
