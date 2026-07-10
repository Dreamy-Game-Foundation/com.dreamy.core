using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Singleton that enforces one instance per scene load.
    /// Destroys duplicates. Set IsPersistent = true (default) for DontDestroyOnLoad.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindAnyObjectByType<T>();
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

            if (IsPersistent)
                DontDestroyOnLoad(gameObject);
        }

        protected virtual bool IsPersistent => true;

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
