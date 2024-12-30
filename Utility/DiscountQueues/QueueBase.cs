using Models;
using Models.DatabaseRelatedModels;


using System;
using System.Collections.Generic;
using System.Linq;
using Utility.DiscountQueues.Interfaces;

public abstract class QueueBase<T> : IQueueBase<T> where T : IHasId
{
    protected readonly SortedSet<T> _queue;
    private readonly object _lock = new object();

    protected QueueBase(IComparer<T> comparer)
    {
        _queue = new SortedSet<T>(comparer);
    }

    public void Enqueue(T item)
    {
        lock (_lock)
        {
            _queue.Add(item);
        }
    }

    public T? Peek()
    {
        lock (_lock)
        {
            return _queue.FirstOrDefault();
        }
    }

    public T? Dequeue()
    {
        lock (_lock)
        {
            if (_queue.Count == 0) return default;

            var next = _queue.First();
            _queue.Remove(next);
            return next;
        }
    }

    public bool IsEmpty
    {
        get
        {
            lock (_lock)
            {
                return _queue.Count == 0;
            }
        }
    }

    public bool RemoveById(int id)
    {
        lock (_lock)
        {
            var itemToRemove = _queue.FirstOrDefault(item => item.Id == id);
            if (itemToRemove != null)
            {
                _queue.Remove(itemToRemove);
                return true;
            }

            return false;
        }
    }
}