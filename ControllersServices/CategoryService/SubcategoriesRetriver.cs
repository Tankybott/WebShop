
using DataAccess.Repository.IRepository;
using Models;
using Services.CategoryService.Interfaces;

namespace Services.CategoryService
{
    public class SubcategoriesRetriver : ISubcategoriesRetriver
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubcategoriesRetriver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetSubcategoriesAsync(string parentCategoryFilter)
        {
            if (parentCategoryFilter != "all")
            {
                if (string.IsNullOrEmpty(parentCategoryFilter)) //when root category 
                {
                    return await _unitOfWork.Category.GetAllAsync(c => c.ParentCategoryId == 0 || c.ParentCategoryId == null);
                }

                int filterValueInt;
                if (int.TryParse(parentCategoryFilter, out filterValueInt))
                {
                    return await _unitOfWork.Category.GetAllAsync(c => c.ParentCategoryId == filterValueInt);
                }
            }
            return await _unitOfWork.Category.GetAllAsync();
        }
    }
}
