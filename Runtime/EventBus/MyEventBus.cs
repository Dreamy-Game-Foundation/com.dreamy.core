using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Generic static event bus. Uses struct events (zero allocation).
    /// Safe against add/remove during Raise (deferred mutation).
    /// </summary>
    /// <example>
    /// // Define event
    /// public struct PlayerDiedEvent : IEvent { public int Score; }
    ///
    /// // Subscribe (in MonoBehaviour)
    /// EventBinding&lt;PlayerDiedEvent&gt; _binding;
    /// void OnEnable()  => MyEventBus&lt;PlayerDiedEvent&gt;.Register(_binding = new EventBinding&lt;PlayerDiedEvent&gt;(OnPlayerDied));
    /// void OnDisable() => MyEventBus&lt;PlayerDiedEvent&gt;.Unregister(_binding);
    ///
    /// // Fire
    /// MyEventBus&lt;PlayerDiedEvent&gt;.Raise(new PlayerDiedEvent { Score = 100 });
    /// </example>
    public static class MyEventBus<T> where T : struct, IEvent
    {
        private static readonly HashSet<EventBinding<T>> _bindings = new();
        private static readonly List<EventBinding<T>> _pendingAdd = new();
        private static readonly List<EventBinding<T>> _pendingRemove = new();
        private static int _raiseDepth;
        private static bool _clearPending;

        public static void Register(EventBinding<T> binding)
        {
            if (binding == null) return;
            if (_raiseDepth > 0)
                _pendingAdd.Add(binding);
            else
                _bindings.Add(binding);
        }

        public static void Unregister(EventBinding<T> binding)
        {
            if (binding == null) return;
            if (_raiseDepth > 0)
                _pendingRemove.Add(binding);
            else
                _bindings.Remove(binding);
        }

        public static void Raise(T @event)
        {
            _raiseDepth++;
            try
            {
                foreach (var binding in _bindings)
                {
                    try
                    {
                        binding.Invoke(@event);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }
            finally
            {
                _raiseDepth--;
                if (_raiseDepth == 0)
                {
                    ApplyPendingChanges();
                }
            }
        }

        /// <summary>Removes all bindings. Use in test teardown or scene resets.</summary>
        public static void Clear()
        {
            _pendingAdd.Clear();
            _pendingRemove.Clear();

            if (_raiseDepth > 0)
            {
                _clearPending = true;
                return;
            }

            _bindings.Clear();
            _clearPending = false;
        }

        private static void ApplyPendingChanges()
        {
            if (_clearPending)
            {
                _bindings.Clear();
                _pendingAdd.Clear();
                _pendingRemove.Clear();
                _clearPending = false;
                return;
            }

            foreach (EventBinding<T> binding in _pendingRemove)
            {
                _bindings.Remove(binding);
            }

            _pendingRemove.Clear();

            foreach (EventBinding<T> binding in _pendingAdd)
            {
                _bindings.Add(binding);
            }

            _pendingAdd.Clear();
        }
    }
}
