using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

namespace Utility.Queues;
public class ActivationDiscountQueue : SortedQueue<Discount>, IActivationDiscountQueue
{
    public ActivationDiscountQueue()
        : base(Comparer<Discount>.Create((x, y) =>
        {
            var result = x.StartTime.CompareTo(y.StartTime);
            return result != 0 ? result : x.Id.CompareTo(y.Id);
        }))
    {
    }
}