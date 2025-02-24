using Models.ViewModels;

namespace Services.CategoryService.Interfaces
{
    public interface ICategoryVmRetriver
    {
        Task<CategoryVM> GetVMAsync(int? id = null, int? bindedParentId = null);
    }
}