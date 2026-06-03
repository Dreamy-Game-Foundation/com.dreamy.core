using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Singleton that auto-creates itself if no instance exists in the scene.
    /// The created GameObject is always DontDestroyOnLoad.
    /// </summary>
    public abstract class LiveSingleton<T> : MonoBehaviour where T : LiveSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<T>();
                if (_instance != null) return _instance;

                var go = new GameObject($"[{typeof(T).Name}]");
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
