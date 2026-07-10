using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Hidden MonoBehaviour that bridges Unity lifecycle callbacks to AppLifecycle static events.
    /// Auto-creates itself before any scene loads — no manual setup needed.
    /// </summary>
    [AddComponentMenu("")]
    internal sealed class AppLifecycleMonitor : MonoBehaviour
    {
        private static AppLifecycleMonitor instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance != null)
            {
                return;
            }

            instance = FindAnyObjectByType<AppLifecycleMonitor>();
            if (instance != null)
            {
                return;
            }

            var go = new GameObject("[AppLifecycleMonitor]");
            instance = go.AddComponent<AppLifecycleMonitor>();
            DontDestroyOnLoad(go);
        }

        private void OnApplicationPause(bool pauseStatus) => AppLifecycle.RaisePause(pauseStatus);
        private void OnApplicationFocus(bool hasFocus) => AppLifecycle.RaiseFocus(hasFocus);
        private void OnApplicationQuit() => AppLifecycle.RaiseQuit();

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
