using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

namespace Utility.Queues;
public class DiscountActivationQueue : SortedQueue<Discount>, IDiscountActivationQueue
{
    public DiscountActivationQueue()
        : base(Comparer<Discount>.Create((x, y) =>
        {
            var result = x.StartTime.CompareTo(y.StartTime);
            return result != 0 ? result : x.Id.CompareTo(y.Id);
        }))
    {
    }
}