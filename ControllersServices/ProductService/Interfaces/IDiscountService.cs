using Models.DatabaseRelatedModels;

namespace ControllersServices.ProductService.Interfaces
{
    public interface IDiscountService
    {
        Task<Discount> CreateDiscountAsync(DateTime startDate, DateTime endDate, int percentage);
    }
}