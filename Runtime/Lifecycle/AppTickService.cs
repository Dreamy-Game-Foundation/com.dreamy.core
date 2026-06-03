using System.Collections.Generic;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Drives the ITickable.Tick() loop for non-MonoBehaviour services.
    /// Auto-creates itself before any scene loads — no manual setup needed.
    /// </summary>
    /// <example>
    /// public class MyService : ITickable
    /// {
    ///     public MyService() => AppTickService.Register(this);
    ///     public void Dispose() => AppTickService.Unregister(this);
    ///     public void Tick(float deltaTime) { /* per-frame logic */ }
    /// }
    /// </example>
    [AddComponentMenu("")]
    internal sealed class AppTickService : MonoBehaviour
    {
        private static AppTickService _instance;
        private readonly List<ITickable> _tickables = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var go = new GameObject("[AppTickService]");
            _instance = go.AddComponent<AppTickService>();
            DontDestroyOnLoad(go);
        }

        public static void Register(ITickable tickable)
        {
            if (_instance == null)
            {
                DreamyLog.Error("AppTickService not initialized. Ensure RuntimeInitializeOnLoadMethod ran.");
                return;
            }
            if (!_instance._tickables.Contains(tickable))
                _instance._tickables.Add(tickable);
        }

        public static void Unregister(ITickable tickable) => _instance?._tickables.Remove(tickable);

        private void Update()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < _tickables.Count; i++)
                _tickables[i].Tick(dt);
        }
    }
}
