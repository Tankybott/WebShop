using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

public class DeletionDiscountQueue : QueueBase<Discount>, IDeletionDiscountQueue
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