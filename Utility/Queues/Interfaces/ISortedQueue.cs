
using Models.ContractInterfaces;


public interface ISortedQueue<T> where T : IHasId
{
    bool IsEmpty();

    Task<T?> DequeueAsync();
    Task EnqueueAsync(T item);
    Task<T?> PeekAsync();
    Task<bool> RemoveByIdAsync(int id);
}
