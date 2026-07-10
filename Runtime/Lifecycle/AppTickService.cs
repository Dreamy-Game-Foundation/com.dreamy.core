using System;
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
        private readonly List<ITickable> _pendingAdd = new();
        private readonly List<ITickable> _pendingRemove = new();
        private bool _isTicking;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_instance != null)
            {
                return;
            }

            _instance = FindAnyObjectByType<AppTickService>();
            if (_instance != null)
            {
                return;
            }

            var go = new GameObject("[AppTickService]");
            _instance = go.AddComponent<AppTickService>();
            DontDestroyOnLoad(go);
        }

        public static void Register(ITickable tickable)
        {
            if (tickable == null)
            {
                throw new ArgumentNullException(nameof(tickable));
            }

            if (_instance == null)
            {
                DreamyLog.Error("AppTickService not initialized. Ensure RuntimeInitializeOnLoadMethod ran.");
                return;
            }

            _instance.RegisterInternal(tickable);
        }

        public static void Unregister(ITickable tickable)
        {
            if (tickable == null)
            {
                return;
            }

            _instance?.UnregisterInternal(tickable);
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _isTicking = true;
            try
            {
                for (int i = 0; i < _tickables.Count; i++)
                {
                    try
                    {
                        _tickables[i].Tick(dt);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }
            finally
            {
                _isTicking = false;
                ApplyPendingChanges();
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void RegisterInternal(ITickable tickable)
        {
            if (_isTicking)
            {
                _pendingRemove.Remove(tickable);
                if (!_tickables.Contains(tickable) && !_pendingAdd.Contains(tickable))
                {
                    _pendingAdd.Add(tickable);
                }

                return;
            }

            if (!_tickables.Contains(tickable))
            {
                _tickables.Add(tickable);
            }
        }

        private void UnregisterInternal(ITickable tickable)
        {
            if (_isTicking)
            {
                _pendingAdd.Remove(tickable);
                if (!_pendingRemove.Contains(tickable))
                {
                    _pendingRemove.Add(tickable);
                }

                return;
            }

            _tickables.Remove(tickable);
        }

        private void ApplyPendingChanges()
        {
            foreach (ITickable tickable in _pendingRemove)
            {
                _tickables.Remove(tickable);
            }

            _pendingRemove.Clear();

            foreach (ITickable tickable in _pendingAdd)
            {
                if (!_tickables.Contains(tickable))
                {
                    _tickables.Add(tickable);
                }
            }

            _pendingAdd.Clear();
        }
    }
}
