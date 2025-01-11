using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

namespace ControllersServices.ProductManagement.Interfaces
{
    public interface IDiscountService
    {
        public IActivationDiscountQueue DiscountActivationQueue { get; }
        public IDeletionDiscountQueue DiscountDeletionQueue { get; }
        Task<Discount> CreateDiscountAsync(DateTime startDate, DateTime endDate, int percentage);
        void SetActive(Discount discount);

        Task<Discount> UpdateDiscountAsync(int discountId, DateTime startTime, DateTime endTime, int percentage);
        Task DeleteByIdAsync(int discountId);
    }
}