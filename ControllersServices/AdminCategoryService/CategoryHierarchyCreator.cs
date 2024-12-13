using ControllersServices.AdminCategoryService.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.AdminCategoryService
{
    public class CategoryHierarchyCreator : ICategoryHierarchyCreator
    {
        private IUnitOfWork _unitOfWork;

        public CategoryHierarchyCreator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddSubcategoryToCategoryAsync(Category parentCategory, Category childCategory)
        {
            parentCategory.SubCategories.Add(childCategory);    
            _unitOfWork.Category.Update(parentCategory);
            await _unitOfWork.SaveAsync();  
        }

        public async Task DeleteSubcategoryFromCategoryAsync(Category parentCategory, Category childCategory)
        {
            parentCategory.SubCategories.Remove(childCategory);

            _unitOfWork.Category.Update(parentCategory);
            await _unitOfWork.SaveAsync();
        }


        //public async Task BuildAllCategoriesHierarchy(IEnumerable<Category> allCatregories)
        //{

        //    var rootCategories = allCatregories
        //        .Where(c => c.ParentCategoryId == null)
        //        .ToList();

        //    foreach (var rootCategory in rootCategories)
        //    {
        //        BuildSubcategories(rootCategory, allCatregories);
        //    }

        //    _unitOfWork.Category.UpdateRange(rootCategories);
        //    await _unitOfWork.SaveAsync();

        //}
        //private void BuildSubcategories(Category category, IEnumerable<Category> allCategories)
        //{
        //    category.SubCategories = allCategories
        //        .Where(c => c.ParentCategoryId == category.Id)
        //        .ToList();

        //    foreach (var subCategory in category.SubCategories)
        //    {
        //        BuildSubcategories(subCategory, allCategories);
        //    }
        //}
    }
}
