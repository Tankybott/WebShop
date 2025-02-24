using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;
namespace Utility.Queues;
public class DiscountDeletionQueue : SortedQueue<Discount>, IDiscountDeletionQueue
{
    public DiscountDeletionQueue()
        : base(Comparer<Discount>.Create((x, y) =>
        {
            var result = x.EndTime.CompareTo(y.EndTime);
            return result != 0 ? result : x.Id.CompareTo(y.Id);
        }))
    {
    }
}