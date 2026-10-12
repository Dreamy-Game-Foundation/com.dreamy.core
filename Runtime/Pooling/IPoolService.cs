using UnityEngine;

namespace Dreamy.Core.Pooling
{
    /// <summary>Backend-neutral GameObject pooling. The composition root owns the implementation.</summary>
    public interface IPoolService
    {
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);
        T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component;
        GameObject Spawn(GameObject prefab, Transform parent);
        T Spawn<T>(T prefab, Transform parent) where T : Component;
        T Spawn<T>(GameObject prefab, Transform parent) where T : Component;
        T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component;

        /// <summary>Returns null when capacity is exhausted. Configuration runs after OnSpawn; errors return the instance before propagating.</summary>
        GameObject Spawn(GameObject prefab, PoolSpawnOptions options);
        /// <summary>Returns false on capacity exhaustion. Invalid arguments/configuration errors still throw.</summary>
        bool TrySpawn(GameObject prefab, PoolSpawnOptions options, out GameObject instance);
        /// <summary>Delay uses scaled time. Null/already returned instances are ignored; foreign objects are rejected.</summary>
        void Despawn(GameObject instance, float delay = 0f);
        /// <summary>count is the target total clones, capacity 0 is unlimited. persistent applies to the pool, not scene-parented active clones.</summary>
        void Preload(GameObject prefab, int count, int capacity = 0, bool persistent = false);
        /// <summary>Returns active clones and cancels delayed returns only within this service.</summary>
        void DespawnAll();
    }
}
