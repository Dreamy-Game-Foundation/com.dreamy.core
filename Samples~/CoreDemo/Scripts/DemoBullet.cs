using UnityEngine;
using Dreamy.Core;

namespace Dreamy.Core.Samples
{
    /// <summary>
    /// Demo poolable bullet.
    /// Kế thừa PoolableMonoBehaviour để có OnSpawn/OnDespawn hooks.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class DemoBullet : PoolableMonoBehaviour
    {
        Rigidbody _rb;
        GameObjectPool<DemoBullet> _pool;
        float _lifetime;

        void Awake() => _rb = GetComponent<Rigidbody>();

        public void Launch(GameObjectPool<DemoBullet> pool, Vector3 velocity)
        {
            _pool = pool;
            _rb.linearVelocity = velocity;
            _lifetime = 3f;
        }

        public override void OnSpawn()
        {
            _lifetime = 0f;
            _rb.linearVelocity = Vector3.zero;
        }

        public override void OnDespawn()
        {
            _rb.linearVelocity = Vector3.zero;
        }

        void Update()
        {
            _lifetime -= Time.deltaTime;
            if (_lifetime > 0f && _lifetime <= 3f - 0.01f) // đã launch
            {
                if (_lifetime <= 0f) ReturnToPool();
            }
        }

        void OnCollisionEnter(Collision _) => ReturnToPool();

        void ReturnToPool() => _pool?.Despawn(this);
    }
}
