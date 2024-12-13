using ControllersServices.AdminCategoryService.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace ControllersServices.AdminCategoryService
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAdminCategoryVMCreator _adminCategoryVMCreator;
        private readonly ICategoryHierarchyCreator _categoryHierarchyCreator;
        private readonly ICategoryHierarchyRetriver _categoryHierarchyRetriver;

        public AdminCategoryService(
            ICategoryHierarchyCreator categoryHierarchyCreator,
            IUnitOfWork unitOfWork,
            IAdminCategoryVMCreator adminCategoryVMCreator,
            ICategoryHierarchyRetriver categoryHierarchyRetriver)
        {
            _categoryHierarchyCreator = categoryHierarchyCreator;
            _unitOfWork = unitOfWork;
            _adminCategoryVMCreator = adminCategoryVMCreator;
            _categoryHierarchyRetriver = categoryHierarchyRetriver;
        }
        public async Task<AdminCategoryVM> GetAdminCategoryVMAsync(int? id = null, int? bindedParentId = null) 
        {
            var categories = await _unitOfWork.Category.GetAllAsync(tracked: true , sortBy: c => c.Name);

            AdminCategoryVM categoryVM = _adminCategoryVMCreator.CreateCategoryVM(categories);
            {

            if (id != null)
                categoryVM.Category = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            }

            if (bindedParentId != null)
            {
               await BindParentCategoryAsync(categoryVM, bindedParentId);
            }

            return categoryVM;
        }

        private async Task BindParentCategoryAsync(AdminCategoryVM categoryVM, int? bindedParentId)
        {
            if (bindedParentId == 0)
            {
                categoryVM.Category.ParentCategoryId = null;
                categoryVM.BindedParentName = "Root";
            }
            else if (bindedParentId.HasValue)
            {
                var bindedParentCategory = await _unitOfWork.Category.GetAsync(c => c.Id == bindedParentId.Value);
                categoryVM.Category.ParentCategoryId = bindedParentCategory.Id;
                categoryVM.BindedParentName = bindedParentCategory.Name;
            }
        }

        public async Task UpsertAsync(AdminCategoryVM categoryVM) 
        
        {
            categoryVM.AllCategories = await _unitOfWork.Category.GetAllAsync();
            if (categoryVM.Category.Id == 0)
            {
                _unitOfWork.Category.Add(categoryVM.Category);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                _unitOfWork.Category.Update(categoryVM.Category);
                await _unitOfWork.SaveAsync();
                return;
            }

            await BindParentCategoryAsync(categoryVM.Category);
        }

        private async Task BindParentCategoryAsync(Category category)
        {
            if (category.ParentCategoryId != null)
            {
                var parentCategory = await _unitOfWork.Category.GetAsync(c => c.Id == category.ParentCategoryId);
                await _categoryHierarchyCreator.AddSubcategoryToCategoryAsync(parentCategory, category);
            }
        }



        public async Task<IEnumerable<Category>> GetSubcategoriesOfCateogryAsync(string parentCategoryFilter)
        {
            IEnumerable<Category> categories = await _unitOfWork.Category.GetAllAsync();
            if (parentCategoryFilter != "all")
            {
                if (string.IsNullOrEmpty(parentCategoryFilter)) //when root category 
                {
                    return categories.Where(c => c.ParentCategoryId == 0 || c.ParentCategoryId == null);
                }

                int filterValueInt;
                if (int.TryParse(parentCategoryFilter, out filterValueInt))
                {
                    categories = categories.Where(c => c.ParentCategoryId == filterValueInt);
                }
            }
            return categories;
        }

        public async Task DeleteCategoryWithWholeTreeOfSubcategories(int? id)
        {
            if (id == null) 
            {
                throw new ArgumentNullException();
            }  
            else
            {
                var targetCategory = await _unitOfWork.Category.GetAsync(u => u.Id == id, includeProperties: "ParentCategory");
                if (targetCategory.ParentCategoryId != null || targetCategory.ParentCategoryId != 0) 
                {
                    await _categoryHierarchyCreator.DeleteSubcategoryFromCategoryAsync(targetCategory, targetCategory.ParentCategory);
                }
            
                var allSubcategories = await _categoryHierarchyRetriver.GetCollectionOfAllHigherLevelSubcategoriesAsync(targetCategory);
                List<Category> categoriesToBeDeleted = allSubcategories.ToList();
                categoriesToBeDeleted.Add(targetCategory);
                _unitOfWork.Category.RemoveRange(categoriesToBeDeleted);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
