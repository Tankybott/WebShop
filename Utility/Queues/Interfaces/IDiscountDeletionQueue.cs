using Models.DatabaseRelatedModels;


namespace Utility.DiscountQueues.Interfaces
{
    public interface IDiscountDeletionQueue : ISortedQueue<Discount>
    {
    }
}
