using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Dreamy.Core
{
    /// <summary>
    /// Conditional logger. Log/Warn only compile when DREAMY_DEBUG scripting define is active.
    /// Error and exception logs always compile.
    /// </summary>
    public static class DreamyLog
    {
        private const string Prefix = "[Dreamy]";

        [Conditional("DREAMY_DEBUG")]
        public static void Log(object message)
            => Debug.Log($"{Prefix} {message}");

        [Conditional("DREAMY_DEBUG")]
        public static void Log(object message, UnityEngine.Object context)
            => Debug.Log($"{Prefix} {message}", context);

        [Conditional("DREAMY_DEBUG")]
        public static void Warn(object message)
            => Debug.LogWarning($"{Prefix} {message}");

        [Conditional("DREAMY_DEBUG")]
        public static void Warn(object message, UnityEngine.Object context)
            => Debug.LogWarning($"{Prefix} {message}", context);

        public static void Error(object message)
            => Debug.LogError($"{Prefix} {message}");

        public static void Error(object message, UnityEngine.Object context)
            => Debug.LogError($"{Prefix} {message}", context);

        public static void Exception(Exception exception, UnityEngine.Object context = null)
            => Debug.LogException(exception, context);
    }
}
