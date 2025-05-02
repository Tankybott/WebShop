using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Services.DiscountService.Interfaces;
using Utility.DiscountQueues.Interfaces;

namespace Services.DiscountService
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountActivationQueue _discountActivationQueue;
        private readonly IDiscountDeletionQueue _discountDeletionQueue;

        public DiscountService(IUnitOfWork unitOfWork, IDiscountActivationQueue discountActivationQueue, IDiscountDeletionQueue deletionDiscountQueue)
        {
            _unitOfWork = unitOfWork;
            _discountActivationQueue = discountActivationQueue;
            _discountDeletionQueue = deletionDiscountQueue;
        }

        public async Task<Discount> CreateDiscountAsync(DateTime startTime, DateTime endTime, int percentage)
        {
            CheckIfDiscountValid(startTime, endTime, percentage);

            var discount = new Discount
            {
                StartTime = startTime,
                EndTime = endTime,
                Percentage = percentage
            };

            _unitOfWork.Discount.Add(discount);
            await _unitOfWork.SaveAsync();

            if (startTime <= DateTime.Now)
            {
                SetActiveAsync(discount);
                _unitOfWork.Discount.Update(discount);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                await _discountActivationQueue.EnqueueAsync(discount);
            }

            await _discountDeletionQueue.EnqueueAsync(discount);

            return discount;
        }

        public async Task<Discount> UpdateDiscountAsync(int discountId, DateTime startTime, DateTime endTime, int percentage)
        {
            CheckIfDiscountValid(startTime, endTime, percentage);

            var discountToDelete = await _unitOfWork.Discount.GetAsync(d => d.Id == discountId);
            if (discountToDelete != null)
            {
                _unitOfWork.Discount.Remove(discountToDelete);
                await _discountActivationQueue.RemoveByIdAsync(discountToDelete.Id);
                await _discountDeletionQueue.RemoveByIdAsync(discountToDelete.Id);
            }
            var discount = await CreateDiscountAsync(startTime, endTime, percentage);
            return discount;
        }

        public async void SetActiveAsync(Discount discount)
        {
            discount.isActive = true;
        }

        public async Task DeleteByIdAsync(int discountId)
        {
            await _discountActivationQueue.RemoveByIdAsync(discountId);
            await _discountDeletionQueue.RemoveByIdAsync(discountId);
            var discountToDelete = await _unitOfWork.Discount.GetAsync(d => d.Id == discountId);
            if (discountToDelete != null) _unitOfWork.Discount.Remove(discountToDelete);
            await _unitOfWork.SaveAsync();
        }

        private void CheckIfDiscountValid(DateTime startTime, DateTime endTime, int percentage)
        {
            if (startTime >= endTime || endTime < DateTime.Now || percentage < 0 || percentage > 99) throw new Exception("Invalid discount data");
        }
    }
}
