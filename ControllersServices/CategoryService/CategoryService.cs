using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository.IRepository;
using Models;
using Models.ViewModels;
using Services.CategoryService.Interfaces;


namespace ControllersServices.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryVmRetriver _vmRetriver;
        private readonly ICategoryUpserter _categoryUpserter;
        private readonly ISubcategoriesRetriver _subcategoriesRetriver;
        private readonly ICategoryRemover _categoryRemover;


        public CategoryService(
            ICategoryVmRetriver vmRetriver,
            ICategoryUpserter categoryUpserter,
            ISubcategoriesRetriver subcategoriesRetriver,
            ICategoryRemover categoryRemover)
        {
            _vmRetriver = vmRetriver;
            _categoryUpserter = categoryUpserter;
            _subcategoriesRetriver = subcategoriesRetriver;
            _categoryRemover = categoryRemover;
        }
        public async Task<CategoryVM> GetCategoryVMAsync(int? id = null, int? bindedParentId = null) 
        {
            return await _vmRetriver.GetVMAsync(id, bindedParentId);
        }
        public async Task UpsertCategoryAsync(CategoryVM categoryVM) 
        {
            await _categoryUpserter.UpsertAsync(categoryVM);
        }

        public async Task<IEnumerable<Category>> GetSubcategoriesOfCateogryAsync(string parentCategoryFilter)
        {
            return await _subcategoriesRetriver.GetSubcategoriesAsync(parentCategoryFilter);
        }

        public async Task DeleteCategoryWithAllSubcategoriesAsync(int? id)
        {
            await _categoryRemover.DeleteAsync(id);
        }
    }
}
