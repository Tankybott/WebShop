using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;
namespace Utility.Queues;
public class DeletionDiscountQueue : SortedQueue<Discount>, IDeletionDiscountQueue
{
    public DeletionDiscountQueue()
        : base(Comparer<Discount>.Create((x, y) =>
        {
            var result = x.EndTime.CompareTo(y.EndTime);
            return result != 0 ? result : x.Id.CompareTo(y.Id);
        }))
    {
    }
}