using Models;
using Models.DatabaseRelatedModels;
using System.Threading;
using Utility.DiscountQueues.Interfaces;

public interface IQueueBase<T> where T : IHasId
{
    bool IsEmpty { get; }

    T? Dequeue();
    void Enqueue(T item);
    T? Peek();
    bool RemoveById(int id);
}