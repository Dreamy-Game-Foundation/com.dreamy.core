using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Type-safe service locator. Register services by interface type (typeof(T)),
    /// not by concrete type — supports deferred resolution via callback.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();
        private static readonly Dictionary<Type, List<Action<object>>> _pending = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Clear();
        }

        /// <summary>Registers a service. Resolves any pending deferred callbacks immediately.</summary>
        public static void Register<T>(T service) where T : class
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var type = typeof(T);
            _services[type] = service;

            if (_pending.TryGetValue(type, out var callbacks))
            {
                _pending.Remove(type);

                foreach (Action<object> callback in callbacks)
                {
                    try
                    {
                        callback(service);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }

            DreamyLog.Log($"ServiceLocator: registered {type.Name}");
        }

        /// <summary>Gets a registered service. Throws if not found.</summary>
        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;
            throw new InvalidOperationException(
                $"[ServiceLocator] Service not registered: {typeof(T).Name}. Call Register<T>() first.");
        }

        /// <summary>
        /// Gets a service via callback. If not yet registered, the callback is deferred
        /// and called automatically once Register&lt;T&gt;() is invoked.
        /// </summary>
        public static void Get<T>(Action<T> callback) where T : class
        {
            if (callback == null) return;

            if (_services.TryGetValue(typeof(T), out var service))
            {
                callback((T)service);
                return;
            }

            var type = typeof(T);
            if (!_pending.ContainsKey(type))
                _pending[type] = new List<Action<object>>();
            _pending[type].Add(obj => callback((T)obj));
        }

        /// <summary>Tries to get a service. Returns false without throwing if not registered.</summary>
        public static bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }
            service = null;
            return false;
        }

        public static bool IsRegistered<T>() where T : class
            => _services.ContainsKey(typeof(T));

        /// <summary>Removes a service registration. Does NOT dispose the service.</summary>
        public static void Unregister<T>() where T : class
            => _services.Remove(typeof(T));

        /// <summary>Clears all services and pending callbacks. Intended for testing.</summary>
        public static void Clear()
        {
            _services.Clear();
            _pending.Clear();
        }
    }
}
