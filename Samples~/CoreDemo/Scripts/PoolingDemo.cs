using System;
using Dreamy.Core.Pooling;
using UnityEngine;

namespace Dreamy.Core.Samples
{
    /// <summary>Inject a host-owned backend before wiring SpawnExample/ReturnExample to UI buttons.</summary>
    public sealed class PoolingDemo : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform spawnParent;

        private IPoolService pool;
        private GameObject instance;

        public void Initialize(IPoolService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            ReturnExample();
            pool = service;
        }

        public void SpawnExample()
        {
            if (pool == null) throw new InvalidOperationException("Inject the host's IPoolService first.");
            ReturnExample();
            instance = pool.SpawnLocal(prefab, spawnParent);
        }

        public void ReturnExample()
        {
            if (instance && pool != null) pool.Despawn(instance);
            instance = null;
        }

        public void Shutdown()
        {
            ReturnExample();
            pool = null;
        }

        private void OnDisable() => ReturnExample();
    }
}
