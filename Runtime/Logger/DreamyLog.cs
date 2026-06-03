using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Dreamy.Core
{
    /// <summary>
    /// Conditional logger. Log/Warn only compile when DREAMY_DEBUG scripting define is active.
    /// Error always compiles — errors should never be silently discarded.
    /// Add DREAMY_DEBUG in Project Settings → Player → Scripting Define Symbols for debug builds.
    /// </summary>
    public static class DreamyLog
    {
        [Conditional("DREAMY_DEBUG")]
        public static void Log(object message)
            => Debug.Log($"<color=#00ff88>[Dreamy]</color> {message}");

        [Conditional("DREAMY_DEBUG")]
        public static void Warn(object message)
            => Debug.LogWarning($"[Dreamy] {message}");

        public static void Error(object message)
            => Debug.LogError($"[Dreamy] {message}");
    }
}
