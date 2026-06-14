using System;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Centralized application lifecycle events.
    /// Avoids multiple MonoBehaviours each listening to OnApplicationPause separately.
    /// Auto-initialized at startup via AppLifecycleMonitor.
    /// </summary>
    public static class AppLifecycle
    {
        /// <summary>Fires when app is paused (true) or resumed (false).</summary>
        public static event Action<bool> OnPause;

        /// <summary>Fires when app gains (true) or loses (false) focus.</summary>
        public static event Action<bool> OnFocus;

        /// <summary>Fires just before the application quits.</summary>
        public static event Action OnQuit;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            OnPause = null;
            OnFocus = null;
            OnQuit = null;
        }

        internal static void RaisePause(bool paused) => OnPause?.Invoke(paused);
        internal static void RaiseFocus(bool hasFocus) => OnFocus?.Invoke(hasFocus);
        internal static void RaiseQuit() => OnQuit?.Invoke();
    }
}
