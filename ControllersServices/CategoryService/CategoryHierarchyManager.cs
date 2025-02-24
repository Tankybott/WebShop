using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.CategoryService
{
    public class CategoryHierarchyManager : ICategoryHierarchyManager
    {
        private IUnitOfWork _unitOfWork;

        public CategoryHierarchyManager(IUnitOfWork unitOfWork)
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
    }
}
