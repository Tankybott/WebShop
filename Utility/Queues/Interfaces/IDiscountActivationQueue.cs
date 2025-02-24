using Models.DatabaseRelatedModels;

namespace Utility.DiscountQueues.Interfaces
{
    public interface IDiscountActivationQueue : ISortedQueue<Discount>
    {
    }
}
