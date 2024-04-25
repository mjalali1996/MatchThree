using System;
using UnityEngine;
using Zenject;

namespace Common.Components
{
    public class MonoPoolable : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        private IMemoryPool _pool;

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
        }

        public void Dispose()
        {
            _pool.Despawn(this);
        }
    }

    public class MonoPoolable<T> : MonoPoolable where T : MonoPoolable
    {
        public class Factory : PlaceholderFactory<T>
        {
        }

        public class Pool : MonoMemoryPool<IMemoryPool, T>
        {
        }
    }
}