using UnityEngine;

namespace Dreamy.Core
{
    public static class NumberExtension
    {
        public static bool InRange(this float value, float min, float max) => value >= min && value <= max;
        public static bool InRange(this int value, int min, int max) => value >= min && value <= max;

        public static float Clamp01(this float value) => Mathf.Clamp01(value);
        public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);
        public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);

        /// <summary>Converts large numbers to short strings: 1500 → "1.5K", 2500000 → "2.5M"</summary>
        public static string ToShortString(this long number)
        {
            if (number >= 1_000_000_000) return $"{number / 1_000_000_000.0:F1}B";
            if (number >= 1_000_000) return $"{number / 1_000_000.0:F1}M";
            if (number >= 1_000) return $"{number / 1_000.0:F1}K";
            return number.ToString();
        }

        public static string ToShortString(this int number) => ((long)number).ToShortString();

        public static string ToTimerString(this float seconds)
        {
            int m = (int)(seconds / 60);
            int s = (int)(seconds % 60);
            return $"{m:D2}:{s:D2}";
        }
    }
}
