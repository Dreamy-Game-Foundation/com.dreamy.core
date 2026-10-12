using System;
using UnityEngine;

namespace Dreamy.Core.Pooling
{
    /// <summary>Convenience spawn APIs for any IPoolService backend; dependencies remain explicit.</summary>
    public static class PoolServiceExtensions
    {
        public static GameObject Spawn(this IPoolService service, GameObject prefab)
            => service.Spawn(prefab, new PoolSpawnOptions { Space = Space.Self });
        public static T Spawn<T>(this IPoolService service, T prefab) where T : Component
            => service.Spawn(prefab, new PoolSpawnOptions { Space = Space.Self });
        public static T Spawn<T>(this IPoolService service, GameObject prefab) where T : Component
            => service.Spawn<T>(prefab, new PoolSpawnOptions { Space = Space.Self });
        public static GameObject Spawn(this IPoolService service, GameObject prefab, Vector3 position, Transform parent = null)
            => service.Spawn(prefab, new PoolSpawnOptions { Position = position, Parent = parent });
        public static T Spawn<T>(this IPoolService service, T prefab, Vector3 position, Transform parent = null) where T : Component
            => service.Spawn(prefab, new PoolSpawnOptions { Position = position, Parent = parent });
        public static T Spawn<T>(this IPoolService service, GameObject prefab, Vector3 position, Transform parent = null) where T : Component
            => service.Spawn<T>(prefab, new PoolSpawnOptions { Position = position, Parent = parent });

        public static T Spawn<T>(this IPoolService service, T prefab, PoolSpawnOptions options) where T : Component
        {
            if (!prefab) throw new ArgumentNullException(nameof(prefab));
            return service.Spawn<T>(prefab.gameObject, options);
        }

        public static T Spawn<T>(this IPoolService service, GameObject prefab, PoolSpawnOptions options) where T : Component
        {
            RequireComponent<T>(prefab);
            var instance = service.Spawn(prefab, options);
            if (!instance) return null;
            var component = instance.GetComponent<T>();
            if (component) return component;
            service.Despawn(instance);
            throw new InvalidOperationException($"Spawned instance of {prefab.name} no longer contains {typeof(T).Name}.");
        }

        public static GameObject SpawnAt(this IPoolService service, GameObject prefab, Transform target, Transform parent = null)
        {
            if (!target) throw new ArgumentNullException(nameof(target));
            return service.Spawn(prefab, target.position, target.rotation, parent);
        }
        public static T SpawnAt<T>(this IPoolService service, GameObject prefab, Transform target, Transform parent = null) where T : Component
        {
            if (!target) throw new ArgumentNullException(nameof(target));
            return service.Spawn<T>(prefab, target.position, target.rotation, parent);
        }
        public static T SpawnAt<T>(this IPoolService service, T prefab, Transform target, Transform parent = null) where T : Component
        {
            if (!prefab) throw new ArgumentNullException(nameof(prefab));
            return service.SpawnAt<T>(prefab.gameObject, target, parent);
        }

        public static GameObject SpawnLocal(this IPoolService service, GameObject prefab, Transform parent,
            Vector3 localPosition = default, Quaternion? localRotation = null, Vector3? localScale = null)
            => service.Spawn(prefab, PoolSpawnOptions.Local(parent, localPosition, localRotation ?? Quaternion.identity, localScale));
        public static T SpawnLocal<T>(this IPoolService service, GameObject prefab, Transform parent,
            Vector3 localPosition = default, Quaternion? localRotation = null, Vector3? localScale = null) where T : Component
            => service.Spawn<T>(prefab, PoolSpawnOptions.Local(parent, localPosition, localRotation ?? Quaternion.identity, localScale));
        public static T SpawnLocal<T>(this IPoolService service, T prefab, Transform parent,
            Vector3 localPosition = default, Quaternion? localRotation = null, Vector3? localScale = null) where T : Component
            => service.Spawn(prefab, PoolSpawnOptions.Local(parent, localPosition, localRotation ?? Quaternion.identity, localScale));

        /// <summary>Applies anchored position after spawn. Other RectTransform layout fields remain the prefab/consumer's responsibility.</summary>
        public static RectTransform SpawnUi(this IPoolService service, RectTransform prefab, Transform parent,
            Vector2 anchoredPosition = default, Vector3? localScale = null)
        {
            if (!prefab) throw new ArgumentNullException(nameof(prefab));
            return service.Spawn(prefab, new PoolSpawnOptions
            {
                Space = Space.Self, Parent = parent, Position = Vector3.zero, Rotation = Quaternion.identity,
                LocalScale = localScale ?? prefab.localScale,
                Configure = go => go.GetComponent<RectTransform>().anchoredPosition = anchoredPosition
            });
        }

        public static bool TrySpawn(this IPoolService service, GameObject prefab, out GameObject instance)
            => service.TrySpawn(prefab, new PoolSpawnOptions { Space = Space.Self }, out instance);
        public static bool TrySpawn<T>(this IPoolService service, GameObject prefab, PoolSpawnOptions options, out T component) where T : Component
        {
            component = null;
            RequireComponent<T>(prefab);
            if (!service.TrySpawn(prefab, options, out var instance)) return false;
            component = instance.GetComponent<T>();
            if (component) return true;
            service.Despawn(instance);
            throw new InvalidOperationException($"Spawned instance of {prefab.name} no longer contains {typeof(T).Name}.");
        }
        public static bool TrySpawn<T>(this IPoolService service, T prefab, PoolSpawnOptions options, out T component) where T : Component
        {
            if (!prefab) throw new ArgumentNullException(nameof(prefab));
            return service.TrySpawn(prefab.gameObject, options, out component);
        }
        public static void Despawn(this IPoolService service, Component instance, float delay = 0f)
            => service.Despawn(instance ? instance.gameObject : null, delay);
        public static void Preload<T>(this IPoolService service, T prefab, int count, int capacity = 0, bool persistent = false) where T : Component
        {
            if (!prefab) throw new ArgumentNullException(nameof(prefab));
            service.Preload(prefab.gameObject, count, capacity, persistent);
        }
        private static void RequireComponent<T>(GameObject prefab) where T : Component
        {
            if (!prefab) throw new ArgumentNullException(nameof(prefab));
            if (!prefab.GetComponent<T>()) throw new ArgumentException($"Prefab {prefab.name} must contain {typeof(T).Name} on its root.", nameof(prefab));
        }
    }
}
