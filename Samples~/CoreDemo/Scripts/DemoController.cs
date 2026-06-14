using Dreamy.Core;
using UnityEngine;

namespace Dreamy.Core.Samples
{
    /// <summary>
    /// Demo for ServiceLocator and EventBus.
    /// </summary>
    public class DemoController : MonoBehaviour
    {
        private int score;

        private EventBinding<DemoScoreEvent> scoreBinding;

        private void Awake()
        {
            ServiceLocator.Register<IDemoService>(new DemoService());

            scoreBinding = new EventBinding<DemoScoreEvent>(OnScoreChanged);
            MyEventBus<DemoScoreEvent>.Register(scoreBinding);
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
                score += 100;
                MyEventBus<DemoScoreEvent>.Raise(
                    new DemoScoreEvent { Score = score });
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                ServiceLocator.Get<IDemoService>().DoSomething();
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
