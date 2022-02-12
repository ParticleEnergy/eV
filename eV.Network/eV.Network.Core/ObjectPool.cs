// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
namespace eV.Network.Core;

public sealed class ObjectPool<T> where T : class
{
    private readonly ConcurrentStack<T> _pool;

    public ObjectPool()
    {
        _pool = new ConcurrentStack<T>();
    }

    public int Count
    {
        get
        {
            lock (_pool)
            {
                return _pool.Count;
            }
        }
    }

    public void Push(T item)
    {
        lock (_pool)
        {
            _pool.Push(item);
        }
    }

    public T? Pop()
    {
        lock (_pool)
        {
            return _pool.TryPop(out T? item) ? item : null;
        }
    }
}
