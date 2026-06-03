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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var go = new GameObject("[AppLifecycleMonitor]");
            go.AddComponent<AppLifecycleMonitor>();
            DontDestroyOnLoad(go);
        }

        private void OnApplicationPause(bool pauseStatus) => AppLifecycle.RaisePause(pauseStatus);
        private void OnApplicationFocus(bool hasFocus) => AppLifecycle.RaiseFocus(hasFocus);
        private void OnApplicationQuit() => AppLifecycle.RaiseQuit();
    }
}
