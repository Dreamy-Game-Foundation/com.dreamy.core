using UnityEngine;
using UnityEngine.Pool;

namespace Dreamy.Core
{
    /// <summary>
    /// Generic prefab pool backed by Unity's <see cref="ObjectPool{T}"/>.
    /// T must be a Component that implements <see cref="IPoolable"/>.
    /// </summary>
    public class GameObjectPool<T> where T : Component, IPoolable
    {
        readonly ObjectPool<T> _pool;

        public int CountActive   => _pool.CountActive;
        public int CountInactive => _pool.CountInactive;
        public int CountAll      => _pool.CountAll;

        /// <param name="prefab">Source prefab to instantiate from.</param>
        /// <param name="parent">Optional parent transform for pooled objects.</param>
        /// <param name="defaultCapacity">Initial internal list capacity.</param>
        /// <param name="maxSize">Max inactive objects kept alive; extras are destroyed.</param>
        public GameObjectPool(T prefab, Transform parent = null, int defaultCapacity = 10, int maxSize = 200)
        {
            _pool = new ObjectPool<T>(
                createFunc:      () => Object.Instantiate(prefab, parent),
                actionOnGet:     item => { item.gameObject.SetActive(true);  item.OnSpawn();   },
                actionOnRelease: item => { item.OnDespawn(); item.gameObject.SetActive(false); },
                actionOnDestroy: item => Object.Destroy(item.gameObject),
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize:         maxSize
            );
        }

        public T    Spawn()        => _pool.Get();
        public void Despawn(T item) => _pool.Release(item);
        public void Clear()         => _pool.Clear();

        /// <summary>Pre-warms the pool by spawning then immediately despawning <paramref name="count"/> instances.</summary>
        public void Preload(int count)
        {
            var buffer = new T[count];
            for (int i = 0; i < count; i++) buffer[i] = Spawn();
            for (int i = 0; i < count; i++) Despawn(buffer[i]);
        }
    }
}
