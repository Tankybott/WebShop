using Models.DatabaseRelatedModels;

namespace Utility.DiscountQueues.Interfaces
{
    public interface IActivationDiscountQueue : IQueueBase<Discount>
    {
    }
}
