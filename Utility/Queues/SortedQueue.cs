using Models;
using Models.ContractInterfaces;
namespace Utility.Queues;

public abstract class SortedQueue<T> : ISortedQueue<T> where T : IHasId
{
    protected readonly SortedSet<T> _queue;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    protected SortedQueue(IComparer<T> comparer)
    {
        _queue = new SortedSet<T>(comparer);
    }

    public virtual async Task EnqueueAsync(T item)
    {
        await _semaphore.WaitAsync();
        try
        {
            _queue.Add(item);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<T?> PeekAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return _queue.FirstOrDefault();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<T?> DequeueAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_queue.Count == 0) return default;

            var next = _queue.First();
            _queue.Remove(next);
            return next;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> RemoveByIdAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var itemToRemove = _queue.FirstOrDefault(item => item.Id == id);
            if (itemToRemove != null)
            {
                _queue.Remove(itemToRemove);
                return true;
            }

            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool IsEmpty()
    {
        return _queue.Count == 0;
    }
}