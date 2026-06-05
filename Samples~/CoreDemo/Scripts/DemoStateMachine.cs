using UnityEngine;
using Dreamy.Core;

namespace Dreamy.Core.Samples
{
    /// <summary>
    /// Demo StateMachine: Enemy có 3 state (Idle, Chase, Dead).
    /// Hierarchy:
    ///   Enemy (StateMachine component)
    ///   ├── Idle (DemoIdleState component)
    ///   ├── Chase (DemoChaseState component)
    ///   └── Dead (DemoDeadState component)
    /// </summary>
    public class DemoEnemy : MonoBehaviour
    {
        StateMachine _fsm;

        void Awake() => _fsm = GetComponent<StateMachine>();
        void Start()  => _fsm.Initialize<DemoIdleState>();

        // Gọi từ ngoài hoặc từ state khác
        public void StartChase() => _fsm.ChangeState<DemoChaseState>();
        public void Die()         => _fsm.ChangeState<DemoDeadState>();
    }

    // ── States ────────────────────────────────────────────────────────────────────

    public class DemoIdleState : BaseState
    {
        float _timer;

        public override void OnEnter(IStateData data = null)
        {
            _timer = 0f;
            DreamyLog.Log("Enemy → Idle");
        }

        public override void OnExecute()
        {
            _timer += Time.deltaTime;
            // Sau 3 giây tự chuyển sang Chase
            if (_timer >= 3f) Machine.ChangeState<DemoChaseState>();
        }
    }

    public class DemoChaseState : BaseState
    {
        public override void OnEnter(IStateData data = null)
            => DreamyLog.Log("Enemy → Chase");

        public override void OnExecute()
        {
            // Logic chase player...
            // Nhấn K trong DemoController để trigger Dead
        }

        public override void OnExit()
            => DreamyLog.Log("Enemy leaving Chase");
    }

    public class DemoDeadState : BaseState
    {
        public override void OnEnter(IStateData data = null)
        {
            DreamyLog.Log("Enemy → Dead");
            Destroy(gameObject, 2f);
        }
    }
}
