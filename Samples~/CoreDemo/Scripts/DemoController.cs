// CoreDemo — Chạy scene này để xem tất cả tính năng của com.dreamy.core
// Attach script này lên 1 Empty GameObject trong scene demo

using UnityEngine;
using Dreamy.Core;

namespace Dreamy.Core.Samples
{
    /// <summary>
    /// Demo tổng hợp: ServiceLocator + EventBus + BindableProperty.
    /// Attach lên GameObject "DemoController" trong scene.
    /// </summary>
    public class DemoController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] DemoBullet _bulletPrefab;

        // Pool
        GameObjectPool<DemoBullet> _bulletPool;

        // Reactive properties (tương đương score/lives trong game thật)
        readonly BindableProperty<int>   _score = new(0);
        readonly BindableProperty<float> _health = new(100f);

        // EventBus binding tokens
        EventBinding<DemoScoreEvent> _scoreBinding;

        void Awake()
        {
            // 1. Pool — tạo pool 10 bullets
            if (_bulletPrefab != null)
            {
                _bulletPool = new GameObjectPool<DemoBullet>(_bulletPrefab, transform, defaultCapacity: 10);
                _bulletPool.Preload(5);
            }

            // 2. ServiceLocator — đăng ký DemoService
            ServiceLocator.Register<IDemoService>(new DemoService());

            // 3. EventBus — lắng nghe ScoreEvent
            _scoreBinding = new EventBinding<DemoScoreEvent>(OnScoreChanged);
            MyEventBus<DemoScoreEvent>.Register(_scoreBinding);

            // 4. BindableProperty — đăng ký callback, tự cleanup khi GO này bị destroy
            _score.RegisterWithInitValue(v => DreamyLog.Log($"Score: {v}"))
                  .UnRegisterOnDestroy(gameObject);
            _health.RegisterWithInitValue(v => DreamyLog.Log($"Health: {v:F0}"))
                   .UnRegisterOnDestroy(gameObject);
        }

        void OnDestroy()
        {
            MyEventBus<DemoScoreEvent>.Unregister(_scoreBinding);
            ServiceLocator.Unregister<IDemoService>();
        }

        void Update()
        {
            // Space: spawn bullet từ pool
            if (Input.GetKeyDown(KeyCode.Space) && _bulletPool != null)
            {
                var bullet = _bulletPool.Spawn();
                bullet.transform.position = transform.position + Vector3.up;
                bullet.Launch(_bulletPool, Vector3.up * 5f);
            }

            // E: raise EventBus event
            if (Input.GetKeyDown(KeyCode.E))
            {
                _score.Value += 100;
                MyEventBus<DemoScoreEvent>.Raise(new DemoScoreEvent { Score = _score.Value });
            }

            // D: dùng ServiceLocator
            if (Input.GetKeyDown(KeyCode.D))
            {
                var svc = ServiceLocator.Get<IDemoService>();
                svc?.DoSomething();
            }

            // H: giảm health (BindableProperty)
            if (Input.GetKeyDown(KeyCode.H))
                _health.Value = Mathf.Max(0, _health.Value - 10f);
        }

        void OnScoreChanged(DemoScoreEvent e)
        {
            DreamyLog.Log($"[EventBus received] New Score: {e.Score}");
        }
    }

    // ── EventBus event ────────────────────────────────────────────────────────────
    public struct DemoScoreEvent : IEvent
    {
        public int Score;
    }

    // ── ServiceLocator interface + implementation ──────────────────────────────────
    public interface IDemoService
    {
        void DoSomething();
    }

    public class DemoService : IDemoService
    {
        public void DoSomething() => DreamyLog.Log("[DemoService] DoSomething called via ServiceLocator");
    }
}
