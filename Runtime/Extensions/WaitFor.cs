using System.Collections.Generic;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Cached coroutine yield instructions — avoids allocating new WaitForSeconds every call.
    /// </summary>
    public static class WaitFor
    {
        static readonly WaitForFixedUpdate _fixedUpdate = new();
        public static WaitForFixedUpdate FixedUpdate => _fixedUpdate;

        static readonly WaitForEndOfFrame _endOfFrame = new();
        public static WaitForEndOfFrame EndOfFrame => _endOfFrame;

        static readonly Dictionary<float, WaitForSeconds> _cache = new(64, new FloatComparer());

        /// <summary>
        /// Returns a cached <see cref="WaitForSeconds"/> for the given duration.
        /// Returns null if seconds is less than one frame duration.
        /// </summary>
        public static WaitForSeconds Seconds(float seconds)
        {
            if (seconds < 1f / Application.targetFrameRate) return null;
            if (!_cache.TryGetValue(seconds, out var wait))
            {
                wait = new WaitForSeconds(seconds);
                _cache[seconds] = wait;
            }
            return wait;
        }

        class FloatComparer : IEqualityComparer<float>
        {
            public bool Equals(float x, float y) => Mathf.Abs(x - y) <= Mathf.Epsilon;
            public int GetHashCode(float obj) => obj.GetHashCode();
        }
    }
}
