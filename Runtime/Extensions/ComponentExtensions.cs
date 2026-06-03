using UnityEngine;

namespace Dreamy.Core
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Returns the object if it exists and has not been destroyed; otherwise returns null.
        /// Prevents Unity's fake-null from causing misleading NullReferenceExceptions.
        /// </summary>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
            => component.gameObject.GetOrAddComponent<T>();

        public static bool TryGetComponentInParent<T>(this Component component, out T result) where T : Component
        {
            result = component.GetComponentInParent<T>();
            return result != null;
        }

        public static void LogWarning(this Component component, object message)
            => Debug.LogWarning($"[{component.GetType().Name}] {message}", component);

        public static void LogError(this Component component, object message)
            => Debug.LogError($"[{component.GetType().Name}] {message}", component);
    }
}
