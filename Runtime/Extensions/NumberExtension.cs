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
        public static int FloorToInt(this float value) => Mathf.FloorToInt(value);
        public static int CeilToInt(this float value) => Mathf.CeilToInt(value);
        public static int RoundToInt(this float value) => Mathf.RoundToInt(value);
        public static float Abs(this float value) => Mathf.Abs(value);
        public static int Abs(this int value) => Mathf.Abs(value);
        public static bool IsNearlyZero(this float value, float tolerance = 0.0001f) => Mathf.Abs(value) <= tolerance;
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax) => MathUtility.Remap(value, fromMin, fromMax, toMin, toMax);

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
            var m = (int)(seconds / 60);
            var s = (int)(seconds % 60);
            return $"{m:D2}:{s:D2}";
        }

        public static string ToTimerString(this int seconds) => ((float)seconds).ToTimerString();
    }
}
