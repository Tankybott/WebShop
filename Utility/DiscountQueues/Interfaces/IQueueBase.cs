
using Models.ContractInterfaces;


public interface IQueueBase<T> where T : IHasId
{
    bool IsEmpty();

    Task<T?> DequeueAsync();
    Task EnqueueAsync(T item);
    Task<T?> PeekAsync();
    Task<bool> RemoveByIdAsync(int id);
}
