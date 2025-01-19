using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Services.PhotoService.Interfaces.DiscountService.Interfaces;
using Utility.DiscountQueues.Interfaces;

namespace Services.PhotoService.Interfaces.DiscountService
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        public IActivationDiscountQueue DiscountActivationQueue { get; }
        public IDeletionDiscountQueue DiscountDeletionQueue { get; }

        public DiscountService(IUnitOfWork unitOfWork, IActivationDiscountQueue discountActivationQueue, IDeletionDiscountQueue deletionDiscountQueue)
        {
            _unitOfWork = unitOfWork;
            DiscountActivationQueue = discountActivationQueue;
            DiscountDeletionQueue = deletionDiscountQueue;
        }

        public async Task<Discount> CreateDiscountAsync(DateTime startTime, DateTime endTime, int percentage)
        {
            checkIfDiscountValid(startTime, endTime, percentage);

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
                SetActive(discount);
            }
            else
            {
                await DiscountActivationQueue.EnqueueAsync(discount);
            }

            await DiscountDeletionQueue.EnqueueAsync(discount);

            return discount;
        }

        public async Task<Discount> UpdateDiscountAsync(int discountId, DateTime startTime, DateTime endTime, int percentage)
        {
            checkIfDiscountValid(startTime, endTime, percentage);

            var discountToDelete = await _unitOfWork.Discount.GetAsync(d => d.Id == discountId);
            if (discountToDelete != null)
            {
                _unitOfWork.Discount.Remove(discountToDelete);
                await DiscountActivationQueue.RemoveByIdAsync(discountToDelete.Id);
                await DiscountDeletionQueue.RemoveByIdAsync(discountToDelete.Id);
            }
            var discount = await CreateDiscountAsync(startTime, endTime, percentage);
            return discount;
        }

        public void SetActive(Discount discount)
        {
            discount.isActive = true;
        }

        public async Task DeleteByIdAsync(int discountId)
        {
            await DiscountActivationQueue.RemoveByIdAsync(discountId);
            await DiscountDeletionQueue.RemoveByIdAsync(discountId);
            var discountToDelete = await _unitOfWork.Discount.GetAsync(d => d.Id == discountId);
            if (discountToDelete != null) _unitOfWork.Discount.Remove(discountToDelete);
            await _unitOfWork.SaveAsync();
        }

        private void checkIfDiscountValid(DateTime startTime, DateTime endTime, int percentage)
        {
            if (startTime >= endTime || endTime < DateTime.Now || percentage < 0 || percentage > 99) throw new Exception("Invalid discount data");
        }
    }
}
