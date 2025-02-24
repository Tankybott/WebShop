using Models;
using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;
using Utility.Queues.Interfaces;

namespace Utility.Queues
{
    public class CartDeletionQueue : SortedQueue<Cart>, ICartDeletionQueue
    {
        public CartDeletionQueue()
            : base(Comparer<Cart>.Create((x, y) =>
            {
                var result = Nullable.Compare(x.ExpiresTo, y.ExpiresTo);
                return result != 0 ? result : x.Id.CompareTo(y.Id);
            }))
        {
        }

        public override async Task EnqueueAsync(Cart item)
        {
            if (item.ExpiresTo.HasValue)
            {
                await base.EnqueueAsync(item);
            }
        }
    }
}
