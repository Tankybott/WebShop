using Models.ViewModels;

namespace Services.CategoryService.Interfaces
{
    public interface ICategoryUpserter
    {
        Task UpsertAsync(CategoryVM categoryVM);
    }
}