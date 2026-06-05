using Dreamy.Core;
using UnityEngine;

namespace Dreamy.Core.Samples
{
    /// <summary>
    /// Demo for ServiceLocator, EventBus, and BindableProperty.
    /// </summary>
    public class DemoController : MonoBehaviour
    {
        private readonly BindableProperty<int> score = new(0);
        private readonly BindableProperty<float> health = new(100f);

        private EventBinding<DemoScoreEvent> scoreBinding;

        private void Awake()
        {
            ServiceLocator.Register<IDemoService>(new DemoService());

            scoreBinding = new EventBinding<DemoScoreEvent>(OnScoreChanged);
            MyEventBus<DemoScoreEvent>.Register(scoreBinding);

            score.RegisterWithInitValue(value => DreamyLog.Log($"Score: {value}"))
                .UnRegisterOnDestroy(gameObject);
            health.RegisterWithInitValue(value => DreamyLog.Log($"Health: {value:F0}"))
                .UnRegisterOnDestroy(gameObject);
        }

        private void OnDestroy()
        {
            MyEventBus<DemoScoreEvent>.Unregister(scoreBinding);
            ServiceLocator.Unregister<IDemoService>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                score.Value += 100;
                MyEventBus<DemoScoreEvent>.Raise(new DemoScoreEvent { Score = score.Value });
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                ServiceLocator.Get<IDemoService>().DoSomething();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                health.Value = Mathf.Max(0, health.Value - 10f);
            }
        }

        private void OnScoreChanged(DemoScoreEvent e)
        {
            DreamyLog.Log($"[EventBus received] New Score: {e.Score}");
        }
    }

    public struct DemoScoreEvent : IEvent
    {
        public int Score;
    }

    public interface IDemoService
    {
        void DoSomething();
    }

    public sealed class DemoService : IDemoService
    {
        public void DoSomething()
        {
            DreamyLog.Log("[DemoService] DoSomething called via ServiceLocator");
        }
    }
}
