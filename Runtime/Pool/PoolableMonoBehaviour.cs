using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Optional convenience base class for pooled MonoBehaviours.
    /// Override <see cref="OnSpawn"/> and <see cref="OnDespawn"/> for setup/teardown logic.
    /// </summary>
    public abstract class PoolableMonoBehaviour : MonoBehaviour, IPoolable
    {
        public virtual void OnSpawn()   { }
        public virtual void OnDespawn() { }
    }
}
