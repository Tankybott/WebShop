using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

namespace Services.PhotoService.Interfaces.DiscountService.Interfaces
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