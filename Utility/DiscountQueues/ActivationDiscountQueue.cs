using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

public class ActivationDiscountQueue : QueueBase<Discount>, IActivationDiscountQueue
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