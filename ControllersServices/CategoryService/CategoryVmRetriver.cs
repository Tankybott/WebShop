using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository.IRepository;
using Models.ViewModels;
using Services.CategoryService.Interfaces;


namespace Services.CategoryService
{
    public class CategoryVmRetriver : ICategoryVmRetriver
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryVMCreator _categoryVMCreator;

        public CategoryVmRetriver(IUnitOfWork unitOfWork, ICategoryVMCreator categoryVMCreator)
        {
            _unitOfWork = unitOfWork;
            _categoryVMCreator = categoryVMCreator;
        }

        public async Task<CategoryVM> GetVMAsync(int? id = null, int? bindedParentId = null)
        {
            /* Tracked: true, so properly retrives whole category tree, not just root categories */
            var categories = await _unitOfWork.Category.GetAllAsync(tracked: true, sortBy: c => c.Name);

            CategoryVM categoryVM = _categoryVMCreator.CreateCategoryVM(categories);
            {
                if (id != null)
                {
                    categoryVM.Category = await _unitOfWork.Category.GetAsync(c => c.Id == id);
                }
            }

            if (bindedParentId != null)
            {
                await BindParentCategoryAsync(categoryVM, bindedParentId);
            }

            return categoryVM;
        }

        private async Task BindParentCategoryAsync(CategoryVM categoryVM, int? bindedParentId)
        {
            if (bindedParentId == 0)
            {
                categoryVM.Category.ParentCategoryId = null;
                categoryVM.BindedParentName = "Root";
            }
            else if (bindedParentId.HasValue)
            {
                var bindedParentCategory = await _unitOfWork.Category.GetAsync(c => c.Id == bindedParentId.Value);
                categoryVM.Category.ParentCategoryId = bindedParentCategory?.Id;
                categoryVM.BindedParentName = bindedParentCategory?.Name;
            }
        }
    }
}
