using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// A persistent singleton that keeps only the NEWEST instance.
    /// Useful when a singleton prefab may be present in multiple scenes loaded additively,
    /// or when DontDestroyOnLoad and scene reloads can create duplicates.
    /// </summary>
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        public static bool HasInstance => _instance != null;

        /// <summary>Time at which this instance was initialized (used to resolve duplicates).</summary>
        public float InitializationTime { get; private set; }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " [Auto]") { hideFlags = HideFlags.HideAndDontSave };
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            InitializationTime = Time.time;
            DontDestroyOnLoad(gameObject);

            // Destroy all older instances; keep this (newest) one.
            var existing = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (var old in existing)
            {
                if (old == this) continue;
                var reg = old.GetComponent<RegulatorSingleton<T>>();
                if (reg != null && reg.InitializationTime < InitializationTime)
                    Destroy(old.gameObject);
            }

            _instance = this as T;
        }
    }
}
